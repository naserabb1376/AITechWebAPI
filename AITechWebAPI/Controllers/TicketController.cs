using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Ticket;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AITechDATA.CustomResponses;

namespace AITechWebAPI.Controllers
{
    [Route("Ticket")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class TicketController : ControllerBase
    {
        ITicketRep _TicketRep;
        ILogRep _logRep;

        public TicketController(ITicketRep TicketRep,ILogRep logRep)
        {
           _TicketRep = TicketRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllTickets_Base")]
        public async Task<ActionResult<TicketListCustomResponse<Ticket>>> GetAllTickets_Base(GetTicketListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketRep.GetAllTicketsAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetTicketById_Base")]
        public async Task<ActionResult<TicketRowCustomResponse<Ticket>>> GetTicketById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketRep.GetTicketByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistTicket_Base")]
        public async Task<ActionResult<BitResultObject>> ExistTicket_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketRep.ExistTicketAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddTicket_Base")]
        public async Task<ActionResult<BitResultObject>> AddTicket_Base(AddEditTicketRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Ticket Ticket = new Ticket()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserId,
                Description= requestBody.Description ??"",
                Subject = requestBody.Subject,
            };
            var result = await _TicketRep.AddTicketAsync(Ticket);
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

        [HttpPut("EditTicket_Base")]
        public async Task<ActionResult<BitResultObject>> EditTicket_Base(AddEditTicketRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _TicketRep.GetTicketByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Ticket Ticket = new Ticket()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                UserId = requestBody.UserId,
                Description = requestBody.Description ?? "",
                Subject = requestBody.Subject,
            };
            result = await _TicketRep.EditTicketAsync(Ticket);
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

        [HttpDelete("DeleteTicket_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteTicket_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TicketRep.RemoveTicketAsync(requestBody.ID);
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
