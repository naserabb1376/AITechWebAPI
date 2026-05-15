using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.PreRegistration;
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
using System.Text.Json;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("PreRegistration")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]

    public class PreRegistrationController : ControllerBase
    {
        IPreRegistrationRep _PreRegistrationRep;
        ISubmitFormRep _SubmitFormRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public PreRegistrationController(IPreRegistrationRep PreRegistrationRep, ISubmitFormRep SubmitFormRep, ILogRep logRep,IMapper mapper)
        {
           _PreRegistrationRep = PreRegistrationRep;
           _SubmitFormRep = SubmitFormRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllPreRegistrations_Base")]
        public async Task<ActionResult<ListResultObject<PreRegistrationVM>>> GetAllPreRegistrations_Base(GetPreRegistrationListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.GetAllPreRegistrationsAsync(requestBody.ForeignKeyId,requestBody.EntityType,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<PreRegistrationVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetPreRegistrationById_Base")]
        public async Task<ActionResult<RowResultObject<PreRegistrationVM>>> GetPreRegistrationById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.GetPreRegistrationByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<PreRegistrationVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistPreRegistration_Base")]
        public async Task<ActionResult<BitResultObject>> ExistPreRegistration_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.ExistPreRegistrationAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddPreRegistration_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> AddPreRegistration_Base(AddEditPreRegistrationRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var dynamicFormValidation = await ValidateDynamicSubmitFormAsync(requestBody);
            if (dynamicFormValidation != null)
            {
                return dynamicFormValidation;
            }

            PreRegistration PreRegistration = new PreRegistration()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email,
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,
                IsActive = requestBody.IsActive,
                PaymentFinished = requestBody.PaymentFinished,
                
                EducationalClass = requestBody.EducationalClass,
                SchoolName = requestBody.SchoolName,
                FavoriteField = requestBody.FavoriteField,
                RecognitionLevel = requestBody.RecognitionLevel,
                ProgrammingSkillLevel = requestBody.ProgrammingSkillLevel,
                SocialAddress = requestBody.SocialAddress,
                FormData = requestBody.FormData ?? "",

                ForeignKeyId = requestBody.ForeignKeyId,
                EntityType = requestBody.EntityType,
                TargetObjName = requestBody.TargetObjName,
                RegistrationDate = requestBody.RegistrationDate.StringToDate().Value,
                OtherLangs = requestBody.OtherLangs ?? "",
                // Description = requestBody.Description,
            };
            var result = await _PreRegistrationRep.AddPreRegistrationAsync(PreRegistration);
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

        private async Task<ActionResult<BitResultObject>?> ValidateDynamicSubmitFormAsync(AddEditPreRegistrationRequestBody requestBody)
        {
            if (string.IsNullOrWhiteSpace(requestBody.FormData))
            {
                return null;
            }

            JsonDocument formDataDocument;
            try
            {
                formDataDocument = JsonDocument.Parse(requestBody.FormData);
            }
            catch
            {
                return DynamicFormBadRequest("داده‌های فرم‌ساز معتبر نیست");
            }

            using (formDataDocument)
            {
                var root = formDataDocument.RootElement;
                var formId = GetLong(root, "formId");
                var formKey = GetString(root, "formKey");

                if (formId <= 0 && string.IsNullOrWhiteSpace(formKey))
                {
                    return null;
                }

                if (!root.TryGetProperty("values", out var valuesElement) || valuesElement.ValueKind != JsonValueKind.Object)
                {
                    return DynamicFormBadRequest("مقادیر فرم‌ساز معتبر نیست");
                }

                var submitForm = await _SubmitFormRep.GetSubmitFormObjAsync(formId, formKey);
                if (!submitForm.Status || submitForm.Result == null || submitForm.Result.ID <= 0)
                {
                    return DynamicFormBadRequest("فرم پیدا نشد یا فعال نیست");
                }

                var allowedFields = submitForm.Result.Fields
                    .Select(x => x.FieldName)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (!allowedFields.Any())
                {
                    return DynamicFormBadRequest("برای این فرم فیلدی تعریف نشده است");
                }

                var submittedValues = valuesElement.EnumerateObject()
                    .ToDictionary(x => x.Name, x => x.Value.ValueKind == JsonValueKind.String ? x.Value.GetString() ?? "" : x.Value.ToString(), StringComparer.OrdinalIgnoreCase);

                var invalidField = submittedValues.Keys.FirstOrDefault(x => !allowedFields.Contains(x));
                if (!string.IsNullOrWhiteSpace(invalidField))
                {
                    return DynamicFormBadRequest($"فیلد «{invalidField}» در این فرم تعریف نشده است");
                }

                using var configDocument = ParseOptionalJson(submitForm.Result.FormConfig);
                var configRoot = configDocument?.RootElement;
                var hiddenFields = GetStringArray(configRoot, "hiddenFields").ToHashSet(StringComparer.OrdinalIgnoreCase);
                var requiredFields = GetStringArray(configRoot, "requiredFields");

                if (!requiredFields.Any())
                {
                    requiredFields = new List<string>() { "studentFullName", "firstName", "lastName", "phoneNumber" };
                }

                var now = DateTime.Now;
                var startAt = GetString(configRoot, "startAt");
                var endAt = GetString(configRoot, "endAt");

                if (!string.IsNullOrWhiteSpace(startAt) && DateTime.TryParse(startAt, out var startDate) && now < startDate)
                {
                    return DynamicFormBadRequest("زمان ثبت این فرم هنوز شروع نشده است");
                }

                if (!string.IsNullOrWhiteSpace(endAt) && DateTime.TryParse(endAt, out var endDate) && now > endDate)
                {
                    return DynamicFormBadRequest("زمان ثبت این فرم به پایان رسیده است");
                }

                foreach (var fieldName in requiredFields.Where(x => allowedFields.Contains(x) && !hiddenFields.Contains(x)))
                {
                    if (fieldName.Equals("studentFullName", StringComparison.OrdinalIgnoreCase))
                    {
                        var hasFullName = HasValue(submittedValues, "studentFullName") ||
                            (!string.IsNullOrWhiteSpace(requestBody.FirstName) && !string.IsNullOrWhiteSpace(requestBody.LastName));

                        if (!hasFullName)
                        {
                            return DynamicFormBadRequest("فیلد نام و نام خانوادگی الزامی است");
                        }

                        continue;
                    }

                    if (!HasValue(submittedValues, fieldName))
                    {
                        var displayName = submitForm.Result.Fields.FirstOrDefault(x => x.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))?.DisplayName ?? fieldName;
                        return DynamicFormBadRequest($"فیلد «{displayName}» الزامی است");
                    }
                }

                var configForeignKeyId = GetLong(configRoot, "foreignKeyId");
                if (configForeignKeyId > 0 && requestBody.ForeignKeyId != configForeignKeyId)
                {
                    return DynamicFormBadRequest("کد رکورد مقصد با تنظیمات فرم مطابقت ندارد");
                }

                if (!string.IsNullOrWhiteSpace(submitForm.Result.EntityName) &&
                    !string.Equals(requestBody.EntityType, submitForm.Result.EntityName, StringComparison.OrdinalIgnoreCase))
                {
                    return DynamicFormBadRequest("نوع موجودیت با تنظیمات فرم مطابقت ندارد");
                }

                if (GetBool(configRoot, "preventDuplicate"))
                {
                    var duplicateKey = GetString(configRoot, "duplicateKey");
                    if (string.IsNullOrWhiteSpace(duplicateKey))
                    {
                        duplicateKey = "phoneNumber";
                    }

                    var duplicateValue = GetDuplicateValue(duplicateKey, requestBody, submittedValues);
                    if (!string.IsNullOrWhiteSpace(duplicateValue))
                    {
                        var exists = await _PreRegistrationRep.ExistsDuplicatePreRegistrationAsync(
                            requestBody.ForeignKeyId,
                            requestBody.EntityType,
                            duplicateKey,
                            duplicateValue,
                            submitForm.Result.FormKey ?? formKey);

                        if (exists)
                        {
                            return DynamicFormBadRequest("این فرم قبلا با همین اطلاعات ثبت شده است");
                        }
                    }
                }
            }

            return null;
        }

        private ActionResult<BitResultObject> DynamicFormBadRequest(string message)
        {
            return BadRequest(new BitResultObject()
            {
                Status = false,
                ErrorMessage = message
            });
        }

        private static JsonDocument? ParseOptionalJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonDocument.Parse(json);
            }
            catch
            {
                return null;
            }
        }

        private static string GetString(JsonElement? element, string propertyName)
        {
            if (element.HasValue && element.Value.ValueKind == JsonValueKind.Object &&
                element.Value.TryGetProperty(propertyName, out var property) &&
                property.ValueKind != JsonValueKind.Null && property.ValueKind != JsonValueKind.Undefined)
            {
                return property.ValueKind == JsonValueKind.String ? property.GetString() ?? "" : property.ToString();
            }

            return "";
        }

        private static long GetLong(JsonElement? element, string propertyName)
        {
            var value = GetString(element, propertyName);
            return long.TryParse(value, out var number) ? number : 0;
        }

        private static bool GetBool(JsonElement? element, string propertyName)
        {
            if (element.HasValue && element.Value.ValueKind == JsonValueKind.Object &&
                element.Value.TryGetProperty(propertyName, out var property) &&
                property.ValueKind != JsonValueKind.Null && property.ValueKind != JsonValueKind.Undefined)
            {
                if (property.ValueKind == JsonValueKind.True) return true;
                if (property.ValueKind == JsonValueKind.False) return false;
                if (property.ValueKind == JsonValueKind.String && bool.TryParse(property.GetString(), out var result)) return result;
            }

            return false;
        }

        private static List<string> GetStringArray(JsonElement? element, string propertyName)
        {
            if (!element.HasValue || element.Value.ValueKind != JsonValueKind.Object ||
                !element.Value.TryGetProperty(propertyName, out var property) ||
                property.ValueKind != JsonValueKind.Array)
            {
                return new List<string>();
            }

            return property.EnumerateArray()
                .Select(x => x.ValueKind == JsonValueKind.String ? x.GetString() ?? "" : x.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        private static bool HasValue(Dictionary<string, string> values, string fieldName)
        {
            return values.TryGetValue(fieldName, out var value) && !string.IsNullOrWhiteSpace(value);
        }

        private static string GetDuplicateValue(string duplicateKey, AddEditPreRegistrationRequestBody requestBody, Dictionary<string, string> submittedValues)
        {
            var key = duplicateKey?.Trim() ?? "";
            switch (key.ToLower())
            {
                case "phonenumber":
                    return requestBody.PhoneNumber ?? "";
                case "email":
                    return requestBody.Email ?? "";
                case "firstname":
                    return requestBody.FirstName ?? "";
                case "lastname":
                    return requestBody.LastName ?? "";
                case "studentfullname":
                    var submittedFullName = submittedValues.TryGetValue("studentFullName", out var fullName) ? fullName : "";
                    return !string.IsNullOrWhiteSpace(submittedFullName)
                        ? submittedFullName
                        : $"{requestBody.FirstName} {requestBody.LastName}".Trim();
                default:
                    return submittedValues.TryGetValue(key, out var value) ? value : "";
            }
        }

        [HttpPut("EditPreRegistration_Base")]
        public async Task<ActionResult<BitResultObject>> EditPreRegistration_Base(AddEditPreRegistrationRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _PreRegistrationRep.GetPreRegistrationByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            PreRegistration PreRegistration = new PreRegistration()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email,
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,
                IsActive = requestBody.IsActive,
                PaymentFinished = requestBody.PaymentFinished,

                EducationalClass = requestBody.EducationalClass,
                SchoolName = requestBody.SchoolName,
                FavoriteField = requestBody.FavoriteField,
                RecognitionLevel = requestBody.RecognitionLevel,
                ProgrammingSkillLevel = requestBody.ProgrammingSkillLevel,
                SocialAddress = requestBody.SocialAddress,
                FormData = requestBody.FormData ?? theRow.Result.FormData ?? "",

                ForeignKeyId = requestBody.ForeignKeyId,
                EntityType = requestBody.EntityType,
                TargetObjName = requestBody.TargetObjName,
                RegistrationDate = requestBody.RegistrationDate.StringToDate().Value,
                ID = requestBody.ID,
                OtherLangs = requestBody.OtherLangs ?? "",
                // Description = requestBody.Description,


            };
            result = await _PreRegistrationRep.EditPreRegistrationAsync(PreRegistration);
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

        [HttpDelete("DeletePreRegistration_Base")]
        public async Task<ActionResult<BitResultObject>> DeletePreRegistration_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.RemovePreRegistrationAsync(requestBody.ID);
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

        [HttpPost("GetRegistrationTypes_Base")]
        public async Task<ActionResult<ListResultObject<PreRegistrationVM>>> GetRegistrationTypes_Base(GetRegistrationTypesListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.GetRegistrationTypesAsync(requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
