using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Authenticate;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.StudentDetails;
using AITechWebAPI.Tools;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("StudentDetails")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class StudentDetailsController : ControllerBase
    {
        IStudentDetailsRep _StudentDetailsRep;
        IParentRep _parentRep;
        ILoginMethodRep _loginRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public StudentDetailsController(IStudentDetailsRep StudentDetailsRep,IParentRep parentRep,ILoginMethodRep loginRep,ILogRep logRep,IMapper mapper)
        {
           _StudentDetailsRep = StudentDetailsRep;
            _parentRep = parentRep;
            _loginRep = loginRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllStudentDetails_Base")]
        public async Task<ActionResult<ListResultObject<StudentDetailsVM>>> GetAllStudentDetails_Base(GetStudentDetailsListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.GetAllStudentDetailsAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<StudentDetailsVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetParentStudents")]
        [AllowAnonymous]
        public async Task<ActionResult<ListResultObject<StudentDetailsVM>>> GetParentStudents(CheckCodeRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            ListResultObject<StudentDetails> result = new ListResultObject<StudentDetails>();
            result.Results = new List<StudentDetails>();

            var checkCode = await CheckSMSCodeInternal(requestBody.PhoneNumber, true, requestBody.VerifyCode);
            if (!checkCode.Status)
            {
                result.ErrorMessage = $"شماره موبایل / کد تایید معتبر نیست";
                result.Status = false;
                return BadRequest(result);
            }
            var parents = await _parentRep.GetAllParentsAsync(pageIndex: 1, pageSize: 0, searchText: requestBody.PhoneNumber);
            result.Results = parents.Results.Select(x => x.StudentDetails).ToList();
            result.Status = parents.Status;

            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<StudentDetailsVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        private async Task<BitResultObject> CheckSMSCodeInternal(string reqMobileNumber, bool reqExists, string reqVerifyCode)
        {
            BitResultObject result = new BitResultObject();

            var validParent = await _parentRep.ExistParentAsync(reqMobileNumber, "phonenumber");

            if (reqExists)
            {
                if (!validParent.Status && string.IsNullOrEmpty(validParent.ErrorMessage))
                {
                    result.Status = false;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) نامعتبر است";
                    return result;
                }
            }
            else
            {
                if (validParent.Status)
                {
                    result.Status = !validParent.Status;
                    result.ErrorMessage = "نام کاربری (شماره موبایل) تکراری است";
                    return result;
                }
            }

            var storedVerifyCode = HttpContext.Session.GetString("VerifyCode") ?? "";
            var loginMethod = await _loginRep.GetAllLoginMethodsAsync(0, 1, 1, reqMobileNumber);
            var loginRecood = loginMethod.Results.FirstOrDefault() ?? new LoginMethod();
            result.Status = await ToolBox.CheckCode(reqMobileNumber, reqVerifyCode, storedVerifyCode, loginRecood);

            if (result.Status)
            {
                result.ErrorMessage = $"کد تایید صحیح است";
                var removeLoginresult = await _loginRep.RemoveLoginMethodAsync(loginRecood.ID);
                if (removeLoginresult.Status)
                {
                    Log log = new Log()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        LogTime = DateTime.Now.ToShamsi(),
                        ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    };
                    await _logRep.AddLogAsync(log);
                }
            }
            else
            {
                result.ErrorMessage = $"کد تایید صحیح نیست";
            }

            return result;
        }

        [HttpPost("GetStudentDetailsById_Base")]
        public async Task<ActionResult<RowResultObject<StudentDetailsVM>>> GetStudentDetailsById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.GetStudentDetailsByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<StudentDetailsVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistStudentDetails_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> ExistStudentDetails_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.ExistStudentDetailsAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddStudentDetails_Base")]
        public async Task<ActionResult<BitResultObject>> AddStudentDetails_Base(AddEditStudentDetailsRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            StudentDetails StudentDetails = new StudentDetails()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserId,
               // Description = requestBody.Description,
            };
            var result = await _StudentDetailsRep.AddStudentDetailsAsync(StudentDetails);
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
            return BadRequest(result);
        }

        [HttpPut("EditStudentDetails_Base")]
        public async Task<ActionResult<BitResultObject>> EditStudentDetails_Base(AddEditStudentDetailsRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _StudentDetailsRep.GetStudentDetailsByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            StudentDetails StudentDetails = new StudentDetails()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = theRow.Result.UserId,
                ID = requestBody.ID,
                // Description = requestBody.Description,


            };
            result = await _StudentDetailsRep.EditStudentDetailsAsync(StudentDetails);
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
            return BadRequest(result);
        }

        [HttpDelete("DeleteStudentDetails_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteStudentDetails_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.RemoveStudentDetailsAsync(requestBody.ID);
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
            return BadRequest(result);
        }
    }
}
