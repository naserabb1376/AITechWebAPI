using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.AdminReport;
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
    [Route("AdminReport")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class AdminReportController : ControllerBase
    {
        IAdminReportRep _AdminReportRep;
        ILogRep _logRep;

        public AdminReportController(IAdminReportRep AdminReportRep,ILogRep logRep)
        {
           _AdminReportRep = AdminReportRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllAdminReports_Base")]
        public async Task<ActionResult<ListResultObject<AdminReport>>> GetAllAdminReports_Base(GetAdminReportListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AdminReportRep.GetAllAdminReportsAsync(requestBody.AdminId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetAdminReportById_Base")]
        public async Task<ActionResult<RowResultObject<AdminReport>>> GetAdminReportById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AdminReportRep.GetAdminReportByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistAdminReport_Base")]
        public async Task<ActionResult<BitResultObject>> ExistAdminReport_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AdminReportRep.ExistAdminReportAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddAdminReport_Base")]
        public async Task<ActionResult<BitResultObject>> AddAdminReport_Base(AddEditAdminReportRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            AdminReport AdminReport = new AdminReport()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                AdminId = requestBody.AdminId,
                Title= requestBody.Title,
                Content = requestBody.Content,
                ReportDate = requestBody.ReportDate.StringToDate(),
            };
            var result = await _AdminReportRep.AddAdminReportAsync(AdminReport);
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

        [HttpPut("EditAdminReport_Base")]
        public async Task<ActionResult<BitResultObject>> EditAdminReport_Base(AddEditAdminReportRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _AdminReportRep.GetAdminReportByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            AdminReport AdminReport = new AdminReport()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                AdminId = requestBody.AdminId,
                Title = requestBody.Title,
                Content = requestBody.Content,
                ReportDate = requestBody.ReportDate.StringToDate(),
            };
            result = await _AdminReportRep.EditAdminReportAsync(AdminReport);
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

        [HttpDelete("DeleteAdminReport_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteAdminReport_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AdminReportRep.RemoveAdminReportAsync(requestBody.ID);
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
