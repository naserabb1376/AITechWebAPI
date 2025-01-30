using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.TicketMessage;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AITechWebAPI.Controllers
{
    [Route("TicketMessage")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class TicketMessageController : ControllerBase
    {
        ITicketMessageRep _TicketMessageRep;
        ILogRep _logRep;

        public TicketMessageController(ITicketMessageRep TicketMessageRep,ILogRep logRep)
        {
           _TicketMessageRep = TicketMessageRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllTicketMessages_Base")]
        public async Task<ActionResult<ListResultObject<TicketMessage>>> GetAllTicketMessages_Base(GetTicketMessageListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketMessageRep.GetAllTicketMessagesAsync(requestBody.UserId,requestBody.TicketId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetTicketMessageById_Base")]
        public async Task<ActionResult<RowResultObject<TicketMessage>>> GetTicketMessageById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketMessageRep.GetTicketMessageByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistTicketMessage_Base")]
        public async Task<ActionResult<BitResultObject>> ExistTicketMessage_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketMessageRep.ExistTicketMessageAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddTicketMessage_Base")]
        public async Task<ActionResult<BitResultObject>> AddTicketMessage_Base(AddEditTicketMessageRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            TicketMessage TicketMessage = new TicketMessage()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                AdminId = requestBody.UserId,
                TicketId= requestBody.TicketId,
                IsAdminResponse = requestBody.IsAdminResponse,
                MessageContent = requestBody.MessageContent
            };
            var result = await _TicketMessageRep.AddTicketMessageAsync(TicketMessage);
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

        [HttpPut("EditTicketMessage_Base")]
        public async Task<ActionResult<BitResultObject>> EditTicketMessage_Base(AddEditTicketMessageRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _TicketMessageRep.GetTicketMessageByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            TicketMessage TicketMessage = new TicketMessage()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                AdminId = requestBody.UserId,
                TicketId = requestBody.TicketId,
                IsAdminResponse = requestBody.IsAdminResponse,
                MessageContent = requestBody.MessageContent,
            };
            result = await _TicketMessageRep.EditTicketMessageAsync(TicketMessage);
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

        [HttpDelete("DeleteTicketMessage_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteTicketMessage_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketMessageRep.RemoveTicketMessageAsync(requestBody.ID);
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
