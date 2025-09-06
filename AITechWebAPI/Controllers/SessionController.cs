using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.Session;
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
    [Route("Session")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class SessionController : ControllerBase
    {
        ISessionRep _SessionRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public SessionController(ISessionRep SessionRep,ILogRep logRep,IMapper mapper)
        {
           _SessionRep = SessionRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllSessions_Base")]
        public async Task<ActionResult<ListResultObject<SessionVM>>> GetAllSessions_Base(GetSessionListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionRep.GetAllSessionsAsync(requestBody.GroupId,requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<SessionVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetSessionById_Base")]
        public async Task<ActionResult<RowResultObject<SessionVM>>> GetSessionById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionRep.GetSessionByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<SessionVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistSession_Base")]
        public async Task<ActionResult<BitResultObject>> ExistSession_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionRep.ExistSessionAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddSession_Base")]
        public async Task<ActionResult<BitResultObject>> AddSession_Base(AddEditSessionRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Session Session = new Session()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                GroupId = requestBody.GroupId,
                SessionDate = requestBody.SessionDate.StringToDate().Value,
            };
            var result = await _SessionRep.AddSessionAsync(Session);
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

        [HttpPut("EditSession_Base")]
        public async Task<ActionResult<BitResultObject>> EditSession_Base(AddEditSessionRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _SessionRep.GetSessionByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Session Session = new Session()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                GroupId = requestBody.GroupId,
                SessionDate = requestBody.SessionDate.StringToDate().Value,
            };
            result = await _SessionRep.EditSessionAsync(Session);
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

        [HttpDelete("DeleteSession_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteSession_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionRep.RemoveSessionAsync(requestBody.ID);
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
