using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.UserCourse;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using AITechWebAPI.ViewModels;

namespace AITechWebAPI.Controllers
{
    [Route("UserCourse")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class UserCourseController : ControllerBase
    {
        IUserCourseRep _UserCourseRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public UserCourseController(IUserCourseRep UserCourseRep,ILogRep logRep,IMapper mapper)
        {
           _UserCourseRep = UserCourseRep;
           _logRep = logRep;
            _mapper = mapper;   
        }

        [HttpPost("GetAllUserCourses_Base")]
        public async Task<ActionResult<ListResultObject<UserCourseVM>>> GetAllUserCourses_Base(GetUserCourseListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserCourseRep.GetAllUserCoursesAsync(requestBody.UserId,requestBody.CourseId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<UserCourseVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetUserCourseById_Base")]
        public async Task<ActionResult<RowResultObject<UserCourseVM>>> GetUserCourseById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserCourseRep.GetUserCourseByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<UserCourseVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistUserCourse_Base")]
        public async Task<ActionResult<BitResultObject>> ExistUserCourse_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserCourseRep.ExistUserCourseAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddUserCourses_Base")]
        public async Task<ActionResult<BitResultObject>> AddUserCourses_Base(List<AddEditUserCourseRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var UserCourses = requestBody.Select(x=> new UserCourse()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = x.UserId,
                CourseId = x.CourseId,
                PeresentType = x.PeresentType,
            }).ToList();
            
            var result = await _UserCourseRep.AddUserCoursesAsync(UserCourses);
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

        [HttpPut("EditUserCourses_Base")]
        public async Task<ActionResult<BitResultObject>> EditUserCourses_Base(List<AddEditUserCourseRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var UserCourses = new List<UserCourse>();

            foreach (var body in requestBody)
            {
                var theRow = await _UserCourseRep.GetUserCourseByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var UserCourse = new UserCourse
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    UserId = body.UserId,
                    CourseId = body.CourseId,
                    PeresentType = body.PeresentType,
                };

                UserCourses.Add(UserCourse);
            }

            result = await _UserCourseRep.EditUserCoursesAsync(UserCourses);
            if (result.Status)
            {
                #region AddLog

                Log log = new Log
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

        [HttpDelete("DeleteUserCourses_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteUserCourses_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _UserCourseRep.RemoveUserCoursesAsync(ids);
            if (result.Status)
            {
                #region AddLog

                Log log = new Log
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
