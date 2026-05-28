using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer;
using AITechDATA.DataLayer.Services;
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
using Microsoft.EntityFrameworkCore;
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
        private readonly IStudentDetailsRep _studentDetailsRep;
        private readonly IParentRep _parentRep;
        private readonly ISettingRep _settingRep;
        private readonly IDiscountRep _discountRep;
        private readonly AITechContext _db;
        private readonly SchoolAITechContext _schoolDb;
        private readonly IMapper _mapper;

        public AuthenticationController(IConfiguration configuration, ILoginMethodRep loginRep, IUserRep userRep, IAddressRep addressRep, ILogRep logRep, ITokenRep tokenRep, IPermissionRep permissionRep, IPermissionRoleRep permissionRole, IStudentDetailsRep studentDetailsRep, IParentRep parentRep, ISettingRep settingRep, IDiscountRep discountRep, AITechContext db, SchoolAITechContext schoolDb, IMapper mapper)
        {
            _configuration = configuration;
            _loginRep = loginRep;
            _userRep = userRep;
            _addressRep = addressRep;
            _logRep = logRep;
            _tokenRep = tokenRep;
            _permissionRep = permissionRep;
            _permissionRoleRep = permissionRole;
            _studentDetailsRep = studentDetailsRep;
            _parentRep = parentRep;
            _settingRep = settingRep;
            _discountRep = discountRep;
            _db = db;
            _schoolDb = schoolDb;
            _mapper = mapper;
        }

        [HttpPost("Authenticate")]
        public async Task<ActionResult<RowResultObject<AuthenticationResultBody>>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            RowResultObject<AuthenticationResultBody> result = new RowResultObject<AuthenticationResultBody>();
            RowResultObject<User> authenticateResult = new RowResultObject<User>();

#if DEBUG
if (authenticationRequestBody.Password == "string")
            {
                authenticationRequestBody = new AuthenticationRequestBody()
                {
                    UserName ="09136857124",
                    Password = "569022mt",
                    LoginType = 1,
                };
            }

#endif

            try
            {
                switch (authenticationRequestBody.LoginType)
                {
                    default:
                    case 1:
                        authenticateResult = await _userRep.AuthenticateAsync(authenticationRequestBody.UserName, authenticationRequestBody.Password, authenticationRequestBody.LoginType);
                        break;
                    case 2:
                        {
                            var validPhoneNumber = await _userRep.ExistUserAsync(authenticationRequestBody.UserName, "username");
                            if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                            {
                                result.Status = validPhoneNumber.Status;
                                result.ErrorMessage = "نام کاربری (شماره تماس) نامعتبر است";
                                return BadRequest(result);
                            }
                            var storedVerifyCode = HttpContext.Session.GetString("VerifyCode") ?? "";
                            var checkCodeResult = await CheckSMSCodeInternal(authenticationRequestBody.UserName, true, authenticationRequestBody.Password);
                            bool validCode = checkCodeResult.Status;
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
                        }
                        break;
                    case 3:
                        {
                            var validPhoneNumber = await _parentRep.ExistParentAsync(authenticationRequestBody.UserName, "phonenumber");
                            if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                            {
                                result.Status = validPhoneNumber.Status;
                                result.ErrorMessage = "نام کاربری (شماره تماس) نامعتبر است";
                                return BadRequest(result);
                            }
                            var storedVerifyCode = HttpContext.Session.GetString("VerifyCode") ?? "";
                            var checkCodeResult = await CheckSMSCodeInternal(authenticationRequestBody.UserName, true, authenticationRequestBody.Password);
                            bool validCode = checkCodeResult.Status;
                            if (validCode)
                            {
                                if (authenticationRequestBody.StudentDetailsId.HasValue && authenticationRequestBody.StudentDetailsId > 0)
                                {
                                    var existStd = await _studentDetailsRep.ExistStudentDetailsAsync(authenticationRequestBody.StudentDetailsId.Value);
                                    if (existStd.Status)
                                    {
                                        var parents = await _parentRep.GetAllParentsAsync(authenticationRequestBody.StudentDetailsId.Value, 1, 0, authenticationRequestBody.UserName);
                                        var parent = parents.Results.FirstOrDefault();
                                        if (parent != null)
                                        {
                                            authenticateResult = await _userRep.AuthenticateAsync(authenticationRequestBody.StudentDetailsId.Value.ToString(), authenticationRequestBody.Password, authenticationRequestBody.LoginType);

                                            authenticateResult.Result.LastName = $"({authenticateResult.Result.FirstName} {authenticateResult.Result.LastName})";
                                            authenticateResult.Result.FirstName = $"{parent.Name}";

                                        }
                                        else
                                        {
                                            result.Status = false;
                                            result.ErrorMessage = "کد دانشجو نامعتبر است";
                                            return BadRequest(result);
                                        }
                                    }
                                    else
                                    {
                                        result.Status = false;
                                        result.ErrorMessage = "کد دانشجو نامعتبر است";
                                        return BadRequest(result);
                                    }
                                }
                                else
                                {
                                    result.Status = false;
                                    result.ErrorMessage = "کد دانشجو نامعتبر است";
                                    return BadRequest(result);
                                }
                            }
                            else
                            {
                                result.Status = validCode;
                                result.ErrorMessage = "کد تایید نامعتبر است";
                                return BadRequest(result);
                            }
                            break;
                        }
                }

                result.Status = authenticateResult.Status;
                result.ErrorMessage = authenticateResult.ErrorMessage;

                if (authenticateResult.Status)
                {
                    if (authenticateResult.Result.StudentDetails == null)
                    {
                        var newStudent = new StudentDetails()
                        {
                            CreateDate = DateTime.Now.ToShamsi(),
                            UpdateDate = DateTime.Now.ToShamsi(),
                            UserId = authenticateResult.Result.ID
                        };

                        await _studentDetailsRep.AddStudentDetailsAsync(newStudent);

                        authenticateResult.Result.StudentDetails = newStudent;
                    }
                    var refreshToken = ToolBox.GenerateToken(); // تولید رفرش توکن
                    //var permissionObj = await _permissionRep.GetAllPermissionsAsync(authenticateResult.Result.RoleId, "action", 1, 0);
                    //var permissionsJson = JsonConvert.SerializeObject(permissionObj.Results.Select(x => x.Routename).ToList()).ToHash();
                    var accessToken = ToolBox.GenerateAccessToken(authenticateResult.Result); // تولید رفرش توکن
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
                            ExpirationDate = DateTime.Now.ToShamsi().AddHours(1),
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
                var permissionObj = await _permissionRep.GetAllPermissionsAsync(user.Result.RoleId, user.Result.ID, "action", 0, "", 1, 0);
                var permissionsJson = JsonConvert.SerializeObject(permissionObj.Results.Select(x => x.Routename).ToList()).ToHash();
                var accessToken = ToolBox.GenerateAccessToken(user.Result); // تولید رفرش توکن
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

        [HttpPost("TakinSchoolRegistration")]
        public async Task<ActionResult<object>> TakinSchoolRegistration(TakinSchoolRegistrationRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = new BitResultObject();
            var phoneNumber = OnlyDigits(requestBody.PhoneNumber);
            var nationalCode = OnlyDigits(requestBody.NationalCode);
            var addressPostalCode = OnlyDigits(requestBody.AddressPostalCode);
            var email = string.IsNullOrWhiteSpace(requestBody.Email)
                ? $"takinschool-{nationalCode}@takinschool.local"
                : requestBody.Email.Trim();
            var password = string.IsNullOrWhiteSpace(requestBody.Password)
                ? $"Takin@{Guid.NewGuid():N}"
                : requestBody.Password.Trim();

            var userNameExists = await _schoolDb.Users.AsNoTracking().AnyAsync(x => x.Username == phoneNumber);
            if (userNameExists)
            {
                result.Status = false;
                result.ErrorMessage = "این شماره موبایل قبلا در سیستم ثبت شده است";
                return BadRequest(result);
            }

            var emailExists = await _schoolDb.Users.AsNoTracking().AnyAsync(x => x.Email == email);
            if (emailExists)
            {
                result.Status = false;
                result.ErrorMessage = "پست الکترونیک تکراری است";
                return BadRequest(result);
            }

            var nationalCodeExists = await _schoolDb.Users.AsNoTracking().AnyAsync(x => x.NationalCode == nationalCode);
            if (nationalCodeExists)
            {
                result.Status = false;
                result.ErrorMessage = "کد ملی تکراری است";
                return BadRequest(result);
            }

            var cityId = requestBody.CityID;
            if (cityId <= 0)
            {
                var defaultCity = await _schoolDb.Cities
                    .AsNoTracking()
                    .OrderByDescending(x => x.DefaultCity)
                    .ThenBy(x => x.ID)
                    .FirstOrDefaultAsync();

                if (defaultCity == null)
                {
                    result.Status = false;
                    result.ErrorMessage = "شهر پیش‌فرض برای ثبت آدرس پیدا نشد";
                    return BadRequest(result);
                }

                cityId = defaultCity.ID;
            }
            else
            {
                var cityExists = await _schoolDb.Cities.AsNoTracking().AnyAsync(x => x.ID == cityId);
                if (!cityExists)
                {
                    result.Status = false;
                    result.ErrorMessage = "شهر انتخاب‌شده معتبر نیست";
                    return BadRequest(result);
                }
            }

            await using var transaction = await _schoolDb.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now.ToShamsi();
                var address = new Address
                {
                    CreateDate = now,
                    UpdateDate = now,
                    CityID = cityId,
                    AddressStreet = requestBody.AddressStreet.Trim(),
                    AddressPostalCode = string.IsNullOrWhiteSpace(addressPostalCode) ? null : addressPostalCode,
                    AddressLocationHorizentalPoint = "",
                    AddressLocationVerticalPoint = ""
                };

                var studentDetails = new StudentDetails
                {
                    CreateDate = now,
                    UpdateDate = now
                };

                var user = new User
                {
                    CreateDate = now,
                    UpdateDate = now,
                    FirstName = requestBody.FirstName.Trim(),
                    LastName = requestBody.LastName.Trim(),
                    Username = phoneNumber,
                    Email = email,
                    NationalCode = nationalCode,
                    PasswordHash = password.ToHash(),
                    RoleId = (long)BaseRole.Student,
                    Address = address,
                    StudentDetails = studentDetails,
                    PermissionsVersion = 1,
                    IsActive = true
                };

                var fatherParent = CreateParent(requestBody.FatherName, requestBody.FatherPhone, requestBody.FatherJob, requestBody.FatherEducation, now);
                var motherParent = CreateParent(requestBody.MotherName, requestBody.MotherPhone, requestBody.MotherJob, requestBody.MotherEducation, now);
                studentDetails.Parents.Add(fatherParent);
                studentDetails.Parents.Add(motherParent);

                await _schoolDb.Users.AddAsync(user);
                await _schoolDb.SaveChangesAsync();

                if (string.IsNullOrWhiteSpace(user.IdentificationCode))
                {
                    user.IdentificationCode = $"AITech{user.ID + 1000}";
                    await _schoolDb.SaveChangesAsync();
                }

                var schoolRegistration = new SchoolRegistration
                {
                    CreateDate = now,
                    UpdateDate = now,
                    UserId = user.ID,
                    StudentDetailsId = studentDetails.ID,
                    FatherParentId = fatherParent.ID,
                    MotherParentId = motherParent.ID,
                    TenantKey = "takinschool",
                    FormType = "elementary-school-registration",
                    CurrentSchoolName = requestBody.CurrentSchoolName.Trim(),
                    TargetGrade = requestBody.TargetGrade.Trim(),
                    RegistrationStatus = "Submitted"
                };
                await _schoolDb.SchoolRegistrations.AddAsync(schoolRegistration);
                await _schoolDb.SaveChangesAsync();

                var log = new Log
                {
                    CreateDate = now,
                    UpdateDate = now,
                    LogTime = now,
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString()
                };
                await _schoolDb.Logs.AddAsync(log);
                await _schoolDb.SaveChangesAsync();
                await transaction.CommitAsync();

                var studentFullName = $"{user.FirstName} {user.LastName}".Trim();
                var registrantMessage = BuildTakinSchoolRegistrantSms(studentFullName);
                var adminMessage = BuildTakinSchoolAdminSms(
                    studentFullName,
                    phoneNumber,
                    requestBody.CurrentSchoolName.Trim(),
                    requestBody.TargetGrade.Trim(),
                    fatherParent.Name,
                    fatherParent.ContactNumber,
                    motherParent.Name);

                try
                {
                    await ToolBox.SendSMSMessage(phoneNumber, registrantMessage);
                    await ToolBox.SendSMSMessage("09133049819", adminMessage);
                }
                catch
                {
                }

                return Ok(new
                {
                    status = true,
                    id = user.ID,
                    userId = user.ID,
                    studentDetailsId = studentDetails.ID,
                    schoolRegistrationId = schoolRegistration.ID,
                    addressId = address.ID,
                    errorMessage = ""
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }

            static Parent CreateParent(string name, string phone, string? job, string? education, DateTime now)
            {
                return new Parent
                {
                    CreateDate = now,
                    UpdateDate = now,
                    Name = name.Trim(),
                    ContactNumber = OnlyDigits(phone),
                    Job = string.IsNullOrWhiteSpace(job) ? null : job.Trim(),
                    Education = string.IsNullOrWhiteSpace(education) ? null : education.Trim()
                };
            }

            static string OnlyDigits(string value)
            {
                return new string((value ?? "").Where(char.IsDigit).ToArray());
            }

            static string BuildTakinSchoolRegistrantSms(string studentFullName)
            {
                return $@"ولی گرامی دانش آموز {studentFullName}
از اعتماد و انتخاب شما برای پیش ثبت نام در دبستان دخترانه تکین صمیمانه سپاسگزاریم.
اطلاعات ثبت شده شما با موفقیت دریافت شد و کارشناسان مجموعه در اولین فرصت جهت هماهنگی و ارائه مشاوره با شما تماس خواهند گرفت.
با تقدیم احترام
دبستان دخترانه تکین؛ جایی برای رشد دخترانی که متفاوت می اندیشند، یاد می گیرند و آینده سازند.";
            }

            static string BuildTakinSchoolAdminSms(string studentFullName, string phoneNumber, string currentSchoolName, string targetGrade, string fatherName, string fatherPhone, string motherName)
            {
                return $@"ثبت نام جدید دبستان دخترانه تکین
دانش آموز: {studentFullName}
شماره تماس: {phoneNumber}
مدرسه قبلی: {currentSchoolName}
پایه ثبت نامی: {targetGrade}
نام پدر: {fatherName}
شماره پدر: {fatherPhone}
نام مادر: {motherName}";
            }
        }

        [HttpPost("Signup")]
        public async Task<ActionResult<BitResultObject>> Signup(SignupRequestBody signupRequestBody)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            BitResultObject result = new BitResultObject();

            Address address = new Address();

            var validUserName = await _userRep.ExistUserAsync(signupRequestBody.UserName, "username");

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

            bool IsvalidInviteCode = false;
            BitResultObject validInviteCode = new BitResultObject();
            if (!string.IsNullOrEmpty(signupRequestBody.InvitationCode))
            {
                validInviteCode = await _userRep.ExistUserAsync(signupRequestBody.InvitationCode, "Identificationcode");

                IsvalidInviteCode = validInviteCode.Status;

                if (!IsvalidInviteCode)
                {
                    result.Status = false;
                    result.ErrorMessage = "کد دعوت نامعتبر است";
                    return BadRequest(result);
                }
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
                    FirstName = signupRequestBody.FirstName,
                    LastName = signupRequestBody.LastName,
                    Username = signupRequestBody.UserName,
                    RoleId = 1,
                    Email = signupRequestBody.Email,
                    NationalCode = signupRequestBody.NationalCode,
                    PasswordHash = signupRequestBody.Password.ToHash(),
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    AddressId = (address != null && address.ID > 0) ? address.ID : null,
                    PermissionsVersion = 1,
                    InviterUserId = IsvalidInviteCode ? validInviteCode.ID : null,
                    IsActive = true,
                };
                //if(user.RoleId == 1) // user is student
                //{
                StudentDetails studentDetails = new StudentDetails()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                };
                user.StudentDetails = studentDetails;
                //}
                result = await _userRep.AddUserAsync(user);


                if (result.Status)
                {
                    if (IsvalidInviteCode)
                    {
                        await AddInvitedUserDiscountsAsync(result.ID, validInviteCode.ID);
                    }

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

        private async Task AddInvitedUserDiscountsAsync(long invitedUserId, long inviterUserId)
        {
            var invitedUserRow = await _userRep.GetUserByIdAsync(invitedUserId);
            var inviterUserRow = await _userRep.GetUserByIdAsync(inviterUserId);
            var invitedUserName = GetUserDisplayName(invitedUserRow.Result, invitedUserId);
            var inviterUserName = GetUserDisplayName(inviterUserRow.Result, inviterUserId);

            var invitedDiscountPercentRow = await _settingRep.GetSettingRowAsync(0, "inviteddiscountpercent");
            var inviteDiscountDurationRow = await _settingRep.GetSettingRowAsync(0, "invitediscountduration");
            var inviteDiscountMaxUsageRow = await _settingRep.GetSettingRowAsync(0, "invitediscountmaxusage");

            if (!invitedDiscountPercentRow.Status || invitedDiscountPercentRow.Result == null ||
                !inviteDiscountDurationRow.Status || inviteDiscountDurationRow.Result == null ||
                !inviteDiscountMaxUsageRow.Status || inviteDiscountMaxUsageRow.Result == null)
            {
                return;
            }

            if (!int.TryParse(invitedDiscountPercentRow.Result.Value, out var invitedDiscountPercent) ||
                !int.TryParse(inviteDiscountDurationRow.Result.Value, out var inviteDiscountDuration) ||
                !int.TryParse(inviteDiscountMaxUsageRow.Result.Value, out var inviteDiscountMaxUsage))
            {
                return;
            }

            var discountCode = $"INVITED-GROUP-{invitedUserId}";
            var existingInvitedDiscounts = await _discountRep.GetAllDiscountsAsync(
                entityName: "group",
                creatorId: invitedUserId,
                pageSize: 0,
                searchText: "");

            if (existingInvitedDiscounts.Results.Any(x =>
                    x.IsActive &&
                    (string.Equals(x.DiscountCode, discountCode, StringComparison.OrdinalIgnoreCase) ||
                     (!string.IsNullOrWhiteSpace(x.Description) &&
                      x.Description.Contains("invitation invited reward", StringComparison.OrdinalIgnoreCase))) &&
                    x.DiscountTargets.Any(t => t.IsActive &&
                                               string.Equals(t.TargetEntityName, "user", StringComparison.OrdinalIgnoreCase) &&
                                               t.TargetId == invitedUserId)))
            {
                return;
            }

            await AddInvitationDiscountAsync(
                invitedUserId,
                "group",
                invitedDiscountPercent,
                inviteDiscountDuration,
                inviteDiscountMaxUsage,
                $"[invitation invited reward] این تخفیف بابت اینکه {invitedUserName} از طرف {inviterUserName} دعوت شده، برای ثبت‌نام گروه درسی {invitedUserName} اعمال می‌شود.",
                discountCode
            );
        }

        private static string GetUserDisplayName(User? user, long fallbackId)
        {
            if (user == null)
            {
                return $"کاربر {fallbackId}";
            }

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName)
                ? (!string.IsNullOrWhiteSpace(user.Username) ? user.Username : $"کاربر {fallbackId}")
                : fullName;
        }

        private async Task<bool> AddOrChargeInvitationDiscountAsync(
            long userId,
            string entity,
            int percent,
            int durationDays,
            int maxUsage,
            string description,
            bool chargeExisting = true,
            string? discountCode = null)
        {
            var discounts = await _discountRep.GetAllDiscountsAsync(entityName: entity, creatorId: userId, pageSize: 0, searchText: "invitation");
            var activeDiscount = discounts.Results
                .Where(x => x.IsActive &&
                            !x.CodeRequired &&
                            x.ExpireDate >= DateTime.Now &&
                            x.DiscountTargets.Any(t => t.IsActive && t.TargetEntityName.ToLower() == "user" && t.TargetId == userId) &&
                            (!chargeExisting || x.DiscountMaxUsage > x.PaymentHistories.Count(p => p.UserId == userId && p.PaymentStatus)))
                .OrderByDescending(x => x.DiscountPercent)
                .FirstOrDefault();

            if (activeDiscount == null)
            {
                return await AddInvitationDiscountAsync(userId, entity, percent, durationDays, maxUsage, description, discountCode);
            }

            if (!chargeExisting)
            {
                return true;
            }

            activeDiscount.DiscountPercent += percent;
            activeDiscount.Description = AppendInvitationDescription(activeDiscount.Description, description);
            activeDiscount.UpdateDate = DateTime.Now.ToShamsi();
            activeDiscount.ExpireDate = DateTime.Now.AddDays(durationDays);
            var result = await _discountRep.EditDiscountAsync(activeDiscount);
            return result.Status;
        }

        private static string AppendInvitationDescription(string? currentDescription, string newDescription)
        {
            if (string.IsNullOrWhiteSpace(currentDescription))
            {
                return newDescription;
            }

            return currentDescription.Contains(newDescription)
                ? currentDescription
                : $"{currentDescription}{Environment.NewLine}{newDescription}";
        }

        private async Task<bool> AddInvitationDiscountAsync(long userId, string entity, int percent, int durationDays, int maxUsage, string description, string? discountCode = null)
        {
            Discount discount = new Discount()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                OtherLangs = null,
                IsActive = true,
                DiscountAmount = 0,
                DiscountCode = string.IsNullOrWhiteSpace(discountCode) ? "".GenerateDiscountCode() : discountCode,
                CodeRequired = false,
                Description = description,
                CreatorId = userId,
                EntityName = entity,
                ForeignKeyId = 0,
                DiscountMaxUsage = maxUsage,
                ExpireDate = DateTime.Now.AddDays(durationDays),
                DiscountPercent = percent,
                DiscountTargets = new List<DiscountTarget>()
                {
                    new DiscountTarget()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        OtherLangs = null,
                        IsActive = true,
                        TargetEntityName = "user",
                        TargetId = userId,
                    }
                },
            };

            var result = await _discountRep.AddDiscountAsync(discount);
            return result.Status;
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
            var validParent = await _parentRep.ExistParentAsync(sendCodeRequestBody.PhoneNumber, "phonenumber");
            if (sendCodeRequestBody.Exists)
            {
                if ((!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage)) && (!validParent.Status && string.IsNullOrEmpty(validParent.ErrorMessage)))
                {
                    result.Status = false;
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



            if (sendCodeResult.SendStatus)
            {
                HttpContext.Session.SetString("VerifyCode", sendCodeResult.Code);
                result.ErrorMessage = $"کد تایید ارسال شد";

                LoginMethod loginMethod = new LoginMethod()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    UserId = null,
                    ExpirationDate = DateTime.Now.AddMinutes(5),
                    Method = "Send VerifyCode",
                    Token = sendCodeResult.Code,
                    MobileNumber = sendCodeRequestBody.PhoneNumber,
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

            if (!ModelState.IsValid)
                return BadRequest(checkCodeRequestBody);

            result = await CheckSMSCodeInternal(checkCodeRequestBody.PhoneNumber, checkCodeRequestBody.Exists, checkCodeRequestBody.VerifyCode);

            if (result.Status)
                return Ok(result);
            else
                return BadRequest(result);
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
                    LoginMethod loginMethod = new LoginMethod()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        UserId = null,
                        ExpirationDate = DateTime.Now.AddMinutes(5),
                        Method = "Send VerifyCode",
                        Token = sendCodeResult.Code,
                        MobileNumber = requestBody.PhoneNumber,
                    };

                    var saveLogin = await _loginRep.AddLoginMethodAsync(loginMethod);

                    if (saveLogin.Status)
                    {
                        result.ErrorMessage = $"کد تایید ارسال شد";
                        return Ok(result);
                    }
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

                            var fullName = $"{user.Result.FirstName} {user.Result.LastName}";
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
                var checkCodeResult = await CheckSMSCodeInternal(requestBody.PhoneNumber, true, requestBody.VerifyCode);

                bool validCode = checkCodeResult.Status;
                result.Status = validCode;


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

            var findToken = await _tokenRep.FindTokenAsync(requestBody.Token, requestBody.TokenType, requestBody.TokenStatus);

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
            BitResultObject result = new BitResultObject() { Status = true, ErrorMessage = "" };

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

        private async Task<BitResultObject> CheckSMSCodeInternal(string reqMobileNumber, bool reqExists, string reqVerifyCode)
        {
            BitResultObject result = new BitResultObject();

            var validPhoneNumber = await _userRep.ExistUserAsync(reqMobileNumber, "username");
            var validParent = await _parentRep.ExistParentAsync(reqMobileNumber, "phonenumber");

            if (reqExists)
            {
                if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage) && (!validParent.Status && string.IsNullOrEmpty(validParent.ErrorMessage)))
                {
                    result.Status = false;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) نامعتبر است";
                    return result;
                }
            }
            else
            {
                if (validPhoneNumber.Status)
                {
                    result.Status = !validPhoneNumber.Status;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) تکراری است";
                    return result;
                }
            }

            //var storedVerifyCode = HttpContext.Session.GetString("VerifyCode") ?? "";
            var loginMethod = await _loginRep.GetLastOtp(reqMobileNumber);
            result.Status = await ToolBox.CheckCode(reqMobileNumber, reqVerifyCode, loginMethod.Result);

            if (result.Status)
            {
                result.ErrorMessage = $"کد تایید صحیح است";
                //var removeLoginresult = await _loginRep.RemoveLoginMethodAsync(loginMethod.Result.ID);
                //if (removeLoginresult.Status)
                //{
                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                };
                await _logRep.AddLogAsync(log);
                //}
            }
            else
            {
                result.ErrorMessage = $"کد تایید صحیح نیست";
            }

            return result;
        }

        [HttpPost("TeacherMembership")]
        public async Task<ActionResult<BitResultObject>> TeacherMembership(SignupRequestBody signupRequestBody)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            BitResultObject result = new BitResultObject();

            Address address = new Address();

            var validUserName = await _userRep.ExistUserAsync(signupRequestBody.UserName, "username");

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
            bool IsvalidInviteCode = false;
            BitResultObject validInviteCode = new BitResultObject();
            if (!string.IsNullOrEmpty(signupRequestBody.InvitationCode))
            {
                validInviteCode = await _userRep.ExistUserAsync(signupRequestBody.InvitationCode, "Identificationcode");

                IsvalidInviteCode = validInviteCode.Status;

                if (!IsvalidInviteCode)
                {
                    result.Status = false;
                    result.ErrorMessage = "کد دعوت نامعتبر است";
                    return BadRequest(result);
                }
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
                    FirstName = signupRequestBody.FirstName,
                    LastName = signupRequestBody.LastName,
                    Username = signupRequestBody.UserName,
                    RoleId = 2,
                    Email = signupRequestBody.Email,
                    NationalCode = signupRequestBody.NationalCode,
                    PasswordHash = signupRequestBody.Password.ToHash(),
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    AddressId = (address != null && address.ID > 0) ? address.ID : null,
                    PermissionsVersion = 1,
                    InviterUserId = IsvalidInviteCode ? validInviteCode.ID : null,
                    IsActive = true,

                };
                ////if(user.RoleId == 1) // user is student
                ////{
                //StudentDetails studentDetails = new StudentDetails()
                //{
                //    CreateDate = DateTime.Now.ToShamsi(),
                //    UpdateDate = DateTime.Now.ToShamsi(),
                //};
                //user.StudentDetails = studentDetails;
                ////}
                result = await _userRep.AddUserAsync(user);

                if (result.Status)
                {
                    if (IsvalidInviteCode)
                    {
                        await AddInvitedUserDiscountsAsync(result.ID, validInviteCode.ID);
                    }

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


    }
}
