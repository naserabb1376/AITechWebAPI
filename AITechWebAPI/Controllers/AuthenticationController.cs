using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models.Authenticate;
using AITechWebAPI.Tools;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AITechWebAPI.Controllers
{
    [Route("Authentication")]
    [ApiController]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginMethodRep _loginRep;
        private readonly IUserRep _userRep;
        private readonly IAddressRep _addressRep;
        private readonly ILogRep _logRep;
        private readonly ITokenRep _tokenRep;
        private readonly IPermissionRep _permissionRep;
        private readonly IPermissionRoleRep _permissionRoleRep;
        private readonly IMapper _mapper;

        public AuthenticationController(IConfiguration configuration,ILoginMethodRep loginRep, IUserRep userRep,IAddressRep addressRep,ILogRep logRep,ITokenRep tokenRep,IPermissionRep permissionRep,IPermissionRoleRep permissionRole,IMapper mapper)
        {
            _configuration = configuration;
            _loginRep = loginRep;
            _userRep = userRep;
            _addressRep = addressRep;
            _logRep = logRep;
            _tokenRep = tokenRep;
            _permissionRep = permissionRep;
            _permissionRoleRep = permissionRole;
            _mapper = mapper;  
        }

        [HttpPost("Authenticate")]
        public async Task<ActionResult<RowResultObject<AuthenticationResultBody>>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            RowResultObject<AuthenticationResultBody> result = new RowResultObject<AuthenticationResultBody>();
            RowResultObject<User> authenticateResult = new RowResultObject<User>();

            try
            {
                switch (authenticationRequestBody.LoginType)
                {
                    default:
                    case 1:
                        authenticateResult = await _userRep.AuthenticateAsync(authenticationRequestBody.UserName, authenticationRequestBody.Password, authenticationRequestBody.LoginType);
                        break;
                    case 2:
                        var validPhoneNumber = await _userRep.ExistUserAsync(authenticationRequestBody.UserName, "username");
                        if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                        {
                            result.Status = validPhoneNumber.Status;
                            result.ErrorMessage = "نام کاربری (شماره تماس) نامعتبر است";
                            return BadRequest(result);
                        }
                        var storedVerifyCode = HttpContext.Session.GetString("VerifyCode") ?? "";
                        bool validCode = await ToolBox.CheckCode(authenticationRequestBody.UserName, authenticationRequestBody.Password,storedVerifyCode);
                        if (validCode)
                        {
                            authenticateResult = await _userRep.AuthenticateAsync(authenticationRequestBody.UserName, authenticationRequestBody.Password, authenticationRequestBody.LoginType);
                        }
                        else
                        {
                            result.Status = validCode;
                            result.ErrorMessage = "کد تایید نامعتبر است";
                            return BadRequest(result);
                        }
                        break;
                }

                result.Status = authenticateResult.Status;
                result.ErrorMessage = authenticateResult.ErrorMessage;

                if (authenticateResult.Status)
                {
                    var refreshToken = ToolBox.GenerateToken(); // تولید رفرش توکن
                    var permissionObj = await _permissionRep.GetAllPermissionsAsync(authenticateResult.Result.RoleId, "action", 1, 0);
                    var permissionsJson = JsonConvert.SerializeObject(permissionObj.Results.Select(x => x.Routename).ToList()).ToHash();
                    var accessToken = ToolBox.GenerateAccessToken(authenticateResult.Result,permissionsJson); // تولید رفرش توکن
                    var refreshTokenExpiryDate = DateTime.Now.ToShamsi().AddDays(30); // تنظیم تاریخ انقضای رفرش توکن برای 30 روز


                    var refreshTokenRecord = new Token
                    {
                        UserId = authenticateResult.Result.ID,
                        TokenValue = refreshToken, // ذخیره رفرش توکن
                        Type = "RefreshToken", // نوع: RefreshToken
                        Status = true,
                        CreatedDate = DateTime.Now.ToShamsi(),
                        ExpiryDate = refreshTokenExpiryDate // تاریخ انقضا
                    };

                    var saverefreshToken = await _tokenRep.AddTokenAsync(refreshTokenRecord);
                    var permissionRoles = await _permissionRoleRep.GetAllPermissionRolesAsync(authenticateResult.Result.RoleId, 0, "menu", 1, 0);
                    if (saverefreshToken.Status)
                    {
                        result.Status = authenticateResult.Status;
                        result.ErrorMessage = authenticateResult.ErrorMessage;
                        result.Result = new AuthenticationResultBody()
                        {
                            RefreshToken = refreshToken, // بازگرداندن رفرش توکن
                            AccessToken = accessToken, // بازگرداندن اکسس توکن
                            User = _mapper.Map<UserVM>(authenticateResult.Result),
                            routename = permissionRoles.Results.Select(x => x.Permission.Routename).ToList(),

                        };

                        LoginMethod loginMethod = new LoginMethod()
                        {
                            CreateDate = DateTime.Now.ToShamsi(),
                            UpdateDate = DateTime.Now.ToShamsi(),
                            UserId = authenticateResult.Result.ID,
                            ExpirationDate= DateTime.Now.ToShamsi().AddHours(1),
                            Method = authenticationRequestBody.LoginType == 1 ? "UserName & Password" : "UserName & VerifyCode",
                            Token = accessToken,
                        };

                        var saveLogin = await _loginRep.AddLoginMethodAsync(loginMethod);
                        if (saveLogin.Status)
                        {

                            #region AddLog
                            Log log = new Log()
                            {
                                CreateDate = DateTime.Now.ToShamsi(),
                                UpdateDate = DateTime.Now.ToShamsi(),
                                LogTime = DateTime.Now.ToShamsi(),
                                ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                            };
                            await _logRep.AddLogAsync(log);
                            #endregion

                           

                            return Ok(result);
                        }
                        else
                        {
                            result.Status = saveLogin.Status;
                            result.ErrorMessage = saveLogin.ErrorMessage;

                            return Ok(result);
                        }

                    }

                    else
                    {
                        result.Status = saverefreshToken.Status;
                        result.ErrorMessage = saverefreshToken.ErrorMessage;
                    }
                }

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message}\n{ex.InnerException?.Message}";
            }


            return BadRequest(result);
        }


        [HttpPost("RefreshToken")]
        public async Task<ActionResult<RowResultObject<RefreshTokenResultBody>>> RefreshToken(RefreshTokenRequestBody requestBody)
        {
            RowResultObject<RefreshTokenResultBody> result = new RowResultObject<RefreshTokenResultBody>();

            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var refreshTokenRecord = await _tokenRep.FindTokenAsync(requestBody.RefreshToken, "RefreshToken");

            if (!refreshTokenRecord.Status && refreshTokenRecord.Result == null)
            {
                result.ErrorMessage = "رفرش توکن نامعتبر است";
                result.Status = false;
                return BadRequest(result);
            }

            var expireTokenResult = await _tokenRep.MakeTokenExpireAsync(refreshTokenRecord.Result.ID);

            if (expireTokenResult.Status)
            {
                var user = await _userRep.GetUserByIdAsync(refreshTokenRecord.Result.UserId);
                var refreshToken = ToolBox.GenerateToken(); // تولید رفرش توکن
                var permissionObj = await _permissionRep.GetAllPermissionsAsync(user.Result.RoleId,"action",1,0);
                var permissionsJson = JsonConvert.SerializeObject(permissionObj.Results.Select(x => x.Routename).ToList()).ToHash();
                var accessToken = ToolBox.GenerateAccessToken(user.Result,permissionsJson); // تولید رفرش توکن
                var refreshTokenExpiryDate = DateTime.Now.ToShamsi().AddDays(30); // تنظیم تاریخ انقضای رفرش توکن برای 30 روز


                var newrefreshTokenRecord = new Token
                {
                    UserId = user.Result.ID,
                    TokenValue = refreshToken, // ذخیره رفرش توکن
                    Type = "RefreshToken", // نوع: RefreshToken
                    Status = true,
                    CreatedDate = DateTime.Now.ToShamsi(),
                    ExpiryDate = refreshTokenExpiryDate // تاریخ انقضا
                };

                var saverefreshToken = await _tokenRep.AddTokenAsync(newrefreshTokenRecord);

                if (saverefreshToken.Status)
                {
                    result.Status = user.Status;
                    result.ErrorMessage = user.ErrorMessage;
                    result.Result = new RefreshTokenResultBody()
                    {
                        RefreshToken = refreshToken, // بازگرداندن رفرش توکن
                        AccessToken = accessToken, // بازگرداندن اکسس توکن
                    };


                    #region AddLog
                    Log log = new Log()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        LogTime = DateTime.Now.ToShamsi(),
                        ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    };
                    await _logRep.AddLogAsync(log);
                    #endregion

                    return Ok(result);
                }
            }
            else
            {
                result.Status = expireTokenResult.Status;
                result.ErrorMessage = expireTokenResult.ErrorMessage;
            }
            return BadRequest(result);
        }




        [HttpPost("Signup")]
        public async Task<ActionResult<BitResultObject>> Signup(SignupRequestBody signupRequestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(signupRequestBody);
            }

            BitResultObject result = new BitResultObject();

            Address address = new Address();

            var validUserName = await _userRep.ExistUserAsync(signupRequestBody.UserName,"username");

            if (validUserName.Status)
            {
                result.Status = !validUserName.Status;
                result.ErrorMessage = "نام کاربری (شماره موبایل) تکراری است";
                return BadRequest(result);
            }

         
            var validEmail = await _userRep.ExistUserAsync(signupRequestBody.Email, "email");

            if (validEmail.Status)
            {
                result.Status = !validEmail.Status;
                result.ErrorMessage = "پست الکترونیک تکراری است";
                return BadRequest(result);
            }

            var validNationalCode = await _userRep.ExistUserAsync(signupRequestBody.NationalCode, "nationalcode");

            if (validNationalCode.Status)
            {
                result.Status = !validNationalCode.Status;
                result.ErrorMessage = "کد ملی تکراری است";
                return BadRequest(result);
            }

            if (signupRequestBody.Address != null)
            {
                address = new Address()
                {
                    CityID = signupRequestBody.Address.CityID,
                    AddressLocationHorizentalPoint = signupRequestBody.Address.AddressLocationHorizentalPoint,
                    AddressLocationVerticalPoint = signupRequestBody.Address.AddressLocationVerticalPoint,
                    AddressPostalCode = signupRequestBody.Address.AddressPostalCode,
                    AddressStreet = signupRequestBody.Address.AddressStreet,
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),

                };

                result = await _addressRep.AddAddressAsync(address);
            }

            if (result.Status)
            {
                User user = new User()
                {
                    FullName = signupRequestBody.FullName,
                    Username = signupRequestBody.UserName,
                    RoleId = 1,
                    Email = signupRequestBody.Email,
                    NationalCode = signupRequestBody.NationalCode,
                    PasswordHash = signupRequestBody.Password.ToHash(),
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    AddressId = (address != null && address.ID > 0) ? address.ID : null,
                };
                if(user.RoleId == 1) // user is student
                {
                    StudentDetails studentDetails = new StudentDetails()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                    };
                    user.StudentDetails = studentDetails;
                }
                result = await _userRep.AddUserAsync(user);

                if (result.Status)
                {
                    #region AddLog

                    Log log = new Log()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        LogTime = DateTime.Now.ToShamsi(),
                        ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

                    };
                    await _logRep.AddLogAsync(log);

                    #endregion

                    return Ok(result);
                }
            }
            return BadRequest(result);
        }

        [HttpPost("SendSMSCode")]
        public async Task<ActionResult<BitResultObject>> SendSMSCode(SendCodeRequestBody sendCodeRequestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(sendCodeRequestBody);
            }

            BitResultObject result = new BitResultObject();

            var validPhoneNumber = await _userRep.ExistUserAsync(sendCodeRequestBody.PhoneNumber, "username");
            if (sendCodeRequestBody.Exists)
            {
                if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                {
                    result.Status = validPhoneNumber.Status;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) نامعتبر است";
                    return BadRequest(result);
                }
            }

            else
            {
                if (validPhoneNumber.Status)
                {
                    result.Status = !validPhoneNumber.Status;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) تکراری است";
                    return BadRequest(result);
                }
            }

            var sendCodeResult = await ToolBox.SendCode(sendCodeRequestBody.PhoneNumber);

            result.Status = sendCodeResult.SendStatus;
            HttpContext.Session.SetString("VerifyCode", sendCodeResult.Code);

            if (result.Status)
            {
                result.ErrorMessage = $"کد تایید ارسال شد";
                return Ok(result);
            }
            result.ErrorMessage = $"در ارسال کد مشکلی بوجود آمد";

            return BadRequest(result);
        }

        [HttpPost("CheckSMSCode")]
        public async Task<ActionResult<BitResultObject>> CheckSMSCode(CheckCodeRequestBody checkCodeRequestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(checkCodeRequestBody);
            }

            BitResultObject result = new BitResultObject();

            var validPhoneNumber = await _userRep.ExistUserAsync(checkCodeRequestBody.PhoneNumber, "username");
            if (checkCodeRequestBody.Exists)
            {
                if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                {
                    result.Status = validPhoneNumber.Status;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) نامعتبر است";
                    return BadRequest(result);
                }
            }

            else
            {
                if (validPhoneNumber.Status)
                {
                    result.Status = !validPhoneNumber.Status;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) تکراری است";
                    return BadRequest(result);
                }
            }

            var storedVerifyCode = HttpContext.Session.GetString("VerifyCode") ?? "";
            result.Status = await ToolBox.CheckCode(checkCodeRequestBody.PhoneNumber,checkCodeRequestBody.VerifyCode, storedVerifyCode);

            if (result.Status)
            {
                result.ErrorMessage = $"کد تایید صحیح است";
                return Ok(result);
            }
            else 
            {
                result.ErrorMessage = $"کد تایید صحیح نیست";
                return BadRequest(result);
            }
        }



        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<RowResultObject<string>>> ForgotPassword(ForgotPasswordRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            RowResultObject<string> result = new RowResultObject<string>();

            if (string.IsNullOrEmpty(requestBody.PhoneNumber) && string.IsNullOrEmpty(requestBody.Email))
            {
                result.Status = false;
                result.ErrorMessage = $"ورود حداقل یکی از مقادیر خواسته شده الزامی است";
            }

            if (!string.IsNullOrEmpty(requestBody.PhoneNumber))
            {
                var validPhoneNumber = await _userRep.ExistUserAsync(requestBody.PhoneNumber, "username");
                if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                {
                    result.Status = validPhoneNumber.Status;
                    result.ErrorMessage = "شماره تماس نامعتبر است";
                    return BadRequest(result);
                }

                var sendCodeResult = await ToolBox.SendCode(requestBody.PhoneNumber);

                result.Status = sendCodeResult.SendStatus;
                HttpContext.Session.SetString("VerifyCode", sendCodeResult.Code);

                if (result.Status)
                {
                    result.ErrorMessage = $"کد تایید ارسال شد";
                    return Ok(result);
                }
            }
            else if (!string.IsNullOrEmpty(requestBody.Email))
            {
                var resetTokenExpiryDate = DateTime.Now.ToShamsi().AddHours(2);

                var existLogin = await _userRep.ExistUserAsync(requestBody.Email, "email");

                if (existLogin.Status)
                {
                    var user = await _userRep.GetUserByIdAsync(existLogin.ID);
                    var resetToken = ToolBox.GenerateToken(user.Result.ID); // تولید رفرش توکن

                    if (user.Status)
                    {

                        var newresetTokenRecord = new Token
                        {
                            UserId = user.Result.ID,
                            TokenValue = resetToken, // ذخیره رفرش توکن
                            Type = "ResetPassword", // نوع: ResetPassword
                            Status = true,
                            CreatedDate = DateTime.Now.ToShamsi(),
                            ExpiryDate = resetTokenExpiryDate // تاریخ انقضا
                        };

                        var saverefreshToken = await _tokenRep.AddTokenAsync(newresetTokenRecord);

                        if (saverefreshToken.Status)
                        {

                            var fullName = user.Result.FullName;
                            var messageText = ToolBox.MakeResetPasswordMessage(fullName, resetToken);
                            bool sentState = ToolBox.SendEmail(requestBody.Email, "بازنشانی کلمه عبور", messageText);

                            #region AddLog
                            Log log = new Log()
                            {
                                CreateDate = DateTime.Now.ToShamsi(),
                                UpdateDate = DateTime.Now.ToShamsi(),
                                LogTime = DateTime.Now.ToShamsi(),
                                ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                            };
                            await _logRep.AddLogAsync(log);
                            #endregion


                            if (sentState)
                            {
                                result.Status = sentState;
                                result.ErrorMessage = $"ایمیلی حاوی لینک بازنشانی رمز عبور برای شما ارسال شد";
                                result.Result = resetToken;
                            }
                            else
                            {
                                result.Status = sentState;
                                result.ErrorMessage = $"در ارسال ایمیلی مشکلی بوجود آمد لطفا دوباره تلاش کنید";
                                result.Result = resetToken;
                            }

                            return Ok(result);
                        }
                        else
                        {
                            result.Status = saverefreshToken.Status;
                            result.ErrorMessage = saverefreshToken.ErrorMessage;
                        }
                    }
                    else
                    {
                        result.Status = user.Status;
                        result.ErrorMessage = user.ErrorMessage;
                    }
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = $"پست الکترونیک {requestBody.Email} در سیستم وجود ندارد";
                }
            }

            return BadRequest(result);
        }


        [HttpPost("ResetPassword")]
        public async Task<ActionResult<BitResultObject>> ResetPassword(ResetPasswordRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            BitResultObject result = new BitResultObject();

            if (string.IsNullOrEmpty(requestBody.VerifyCode) && string.IsNullOrEmpty(requestBody.Token))
            {
                result.Status = false;
                result.ErrorMessage = $"ورود حداقل یکی از مقادیر توکن یا کد تایید الزامی است";
            }

            if (!string.IsNullOrEmpty(requestBody.VerifyCode))
            {
                var validPhoneNumber = await _userRep.ExistUserAsync(requestBody.PhoneNumber, "username");
                if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                {
                    result.Status = validPhoneNumber.Status;
                    result.ErrorMessage = "شماره تماس نامعتبر است";
                    return BadRequest(result);
                }

                var storedVerifyCode = HttpContext.Session.GetString("VerifyCode") ?? "";
                result.Status = await ToolBox.CheckCode(requestBody.PhoneNumber, requestBody.VerifyCode, storedVerifyCode);


                if (result.Status)
                {

                    var existLogin = await _userRep.ExistUserAsync(requestBody.PhoneNumber, "username");

                    if (existLogin.Status)
                    {
                        var user = await _userRep.GetUserByIdAsync(existLogin.ID);

                        if (user.Status)
                        {
                            user.Result.PasswordHash = requestBody.NewPassword.ToHash();

                            var updateuser = await _userRep.EditUserAsync(user.Result);

                            if (updateuser.Status)
                            {
                                result.Status = updateuser.Status;
                                result.ErrorMessage = $"تغییر کلمه عبور با موفقیت انجام شد";

                                #region AddLog
                                Log log = new Log()
                                {
                                    CreateDate = DateTime.Now.ToShamsi(),
                                    UpdateDate = DateTime.Now.ToShamsi(),
                                    LogTime = DateTime.Now.ToShamsi(),
                                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                                };
                                await _logRep.AddLogAsync(log);
                                #endregion

                                return Ok(result);
                            }
                        }

                        else
                        {
                            result.Status = user.Status;
                            result.ErrorMessage = user.ErrorMessage;
                            return BadRequest(result);
                        }

                    }
                    else
                    {
                        result.Status = existLogin.Status;
                        result.ErrorMessage = "شماره موبایل وارد شده در سیستم وجود ندارد";
                        return BadRequest(result);
                    }
                }
                else
                {
                    result.ErrorMessage = $"کد تایید صحیح نیست";
                    return BadRequest(result);
                }
            }
            else if (!string.IsNullOrEmpty(requestBody.Token))
            {
                long userId = long.Parse(requestBody.Token.Split('-')[0]);

                var existLogin = await _userRep.ExistUserAsync(userId.ToString(), "id");

                if (userId > 0 && existLogin.Status)
                {
                    var user = await _userRep.GetUserByIdAsync(userId);

                    if (user.Status)
                    {
                        user.Result.PasswordHash = requestBody.NewPassword.ToHash();

                        var updateduser = await _userRep.EditUserAsync(user.Result);

                        if (updateduser.Status)
                        {
                            result.Status = updateduser.Status;
                            result.ErrorMessage = $"تغییر کلمه عبور با موفقیت انجام شد";

                            #region AddLog
                            Log log = new Log()
                            {
                                CreateDate = DateTime.Now.ToShamsi(),
                                UpdateDate = DateTime.Now.ToShamsi(),
                                LogTime = DateTime.Now.ToShamsi(),
                                ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                            };
                            await _logRep.AddLogAsync(log);
                            #endregion

                            return Ok(result);
                        }
                        else
                        {
                            result.Status = updateduser.Status;
                            result.ErrorMessage = updateduser.ErrorMessage;
                        }
                    }
                    else
                    {
                        result.Status = user.Status;
                        result.ErrorMessage = user.ErrorMessage;
                    }
                }

                else
                {
                    result.Status = false;
                    result.ErrorMessage = "کاربر معتبر نیست";
                }
            }

            return BadRequest(result);
        }

        [HttpPost("CheckToken")]
        public async Task<ActionResult<BitResultObject>> CheckToken(CheckTokenRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            BitResultObject result = new BitResultObject();

            var findToken = await _tokenRep.FindTokenAsync(requestBody.Token,requestBody.TokenType,requestBody.TokenStatus);

            result.Status = findToken.Status;
            result.ErrorMessage = findToken.ErrorMessage;

            if (findToken.Status && findToken.Result != null)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("LogOut")]
        public async Task<ActionResult<BitResultObject>> LogOut(RefreshTokenRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            BitResultObject result = new BitResultObject() { Status = true,ErrorMessage =""};

             var refreshTokenRecord = await _tokenRep.FindTokenAsync(requestBody.RefreshToken, "RefreshToken");

            if (refreshTokenRecord.Status && refreshTokenRecord.Result != null)
            {
                var expireTokenResult = await _tokenRep.MakeTokenExpireAsync(refreshTokenRecord.Result.ID);

                if (expireTokenResult.Status)
                {
                    result.Status = expireTokenResult.Status;
                    result.ErrorMessage = $"کاربر از سیستم خارج شد";

                    #region AddLog
                    Log log = new Log()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        LogTime = DateTime.Now.ToShamsi(),
                        ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    };
                    await _logRep.AddLogAsync(log);
                    #endregion
                }
                else
                {
                    result.Status = expireTokenResult.Status;
                    result.ErrorMessage = expireTokenResult.ErrorMessage;
                    return BadRequest(result);
                }
            }

                return Ok(result);
        }
    }
}