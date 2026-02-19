using AITechDATA.CustomResponses;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Event;
using AITechWebAPI.Models.News;
using AITechWebAPI.Models.Public;
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
    [Route("Event")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]

    public class EventController : ControllerBase
    {
        IEventRep _EventRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public EventController(IEventRep EventRep,ILogRep logRep,IMapper mapper)
        {
           _EventRep = EventRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("GetAllEvents_Base")]
        public async Task<ActionResult<EventListCustomResponse<EventVM>>> GetAllEvents_Base(GetEventListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var clientRoleId = User.GetCurrentRoleId();
            var clientUserId = User.GetCurrentUserId();

            var result = await _EventRep.GetAllEventsAsync(clientUserId,clientRoleId,requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<EventListCustomResponse<EventVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("GetEventById_Base")]
        public async Task<ActionResult<EventRowCustomResponse<EventVM>>> GetEventById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var clientRoleId = User.GetCurrentRoleId();
            var clientUserId = User.GetCurrentUserId();

            var result = await _EventRep.GetEventByIdAsync(requestBody.ID,clientUserId,clientRoleId);
            if (result.Status)
            {
                var resultVM = _mapper.Map<EventRowCustomResponse<EventVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistEvent_Base")]
        public async Task<ActionResult<BitResultObject>> ExistEvent_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EventRep.ExistEventAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddEvent_Base")]
        public async Task<ActionResult<BitResultObject>> AddEvent_Base(AddEditEventRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Event Event = new Event()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserId,
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                EventDate = requestBody.EventDate.StringToDate().Value,
                Keywords = requestBody.Keywords,
                Title = requestBody.Title,
                Fee = requestBody.EventFee,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            var result = await _EventRep.AddEventAsync(Event);
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

        [HttpPut("EditEvent_Base")]
        public async Task<ActionResult<BitResultObject>> EditEvent_Base(AddEditEventRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _EventRep.GetEventByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Event Event = new Event()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                UserId = requestBody.UserId,
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                EventDate = requestBody.EventDate.StringToDate().Value,
                Keywords = requestBody.Keywords,
                Title = requestBody.Title,
                Fee = requestBody.EventFee,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _EventRep.EditEventAsync(Event);
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

        [HttpDelete("DeleteEvent_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteEvent_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EventRep.RemoveEventAsync(requestBody.ID);
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
