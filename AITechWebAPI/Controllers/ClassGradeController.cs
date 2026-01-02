using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.ClassGrade;
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
    [Route("ClassGrade")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class ClassGradeController : ControllerBase
    {
        private IClassGradeRep _ClassGradeRep;
        private ILogRep _logRep;

        public ClassGradeController(IClassGradeRep ClassGradeRep, ILogRep logRep)
        {
            _ClassGradeRep = ClassGradeRep;
            _logRep = logRep;
        }

        [HttpPost("GetAllClassGrades_Base")]
        public async Task<ActionResult<ListResultObject<ClassGrade>>> GetAllClassGrades_Base(GetClassGradeListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ClassGradeRep.GetAllClassGradesAsync(requestBody.EntityName, requestBody.ForeignKeyId, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetClassGradeById_Base")]
        public async Task<ActionResult<RowResultObject<ClassGrade>>> GetClassGradeById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ClassGradeRep.GetClassGradeByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistClassGrade_Base")]
        public async Task<ActionResult<BitResultObject>> ExistClassGrade_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ClassGradeRep.ExistClassGradeAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddClassGrade_Base")]
        public async Task<ActionResult<BitResultObject>> AddClassGrade_Base(AddEditClassGradeRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            ClassGrade ClassGrade = new ClassGrade()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                ForeignKeyId = requestBody.ForeignKeyId,
                EntityName = requestBody.EntityName,
                Description = requestBody.Description ?? "",
                Title = requestBody.Title ?? "",
                GradeScore = requestBody.GradeScore,

            };
            var result = await _ClassGradeRep.AddClassGradeAsync(ClassGrade);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("EditClassGrade_Base")]
        public async Task<ActionResult<BitResultObject>> EditClassGrade_Base(AddEditClassGradeRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _ClassGradeRep.GetClassGradeByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            ClassGrade ClassGrade = new ClassGrade()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                ForeignKeyId = requestBody.ForeignKeyId,
                EntityName = requestBody.EntityName,
                GradeScore = requestBody.GradeScore,
                Description = requestBody.Description ?? "",
                Title = requestBody.Title ?? "",

            };
            result = await _ClassGradeRep.EditClassGradeAsync(ClassGrade);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteClassGrade_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteClassGrade_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ClassGradeRep.RemoveClassGradeAsync(requestBody.ID);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}