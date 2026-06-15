using AITechDATA.DataLayer;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AITechWebAPI.Controllers
{
    [Route("SchoolRegistration")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class SchoolRegistrationController : ControllerBase
    {
        private readonly AITechContext _publicDb;
        private readonly SchoolAITechContext _schoolDb;

        public SchoolRegistrationController(AITechContext publicDb, SchoolAITechContext schoolDb)
        {
            _publicDb = publicDb;
            _schoolDb = schoolDb;
        }

        [HttpPost("GetAllTakinSchoolRegistrations")]
        [AllowAnonymous]
        public async Task<ActionResult<ListResultObject<TakinSchoolRegistrationVM>>> GetAllTakinSchoolRegistrations(GetListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new ListResultObject<TakinSchoolRegistrationVM>();
            var pageIndex = Math.Max(requestBody.PageIndex, 1);
            var pageSize = requestBody.PageSize <= 0 ? 10 : requestBody.PageSize;
            var searchText = (requestBody.SearchText ?? "").Trim();

            try
            {
                var query = _schoolDb.SchoolRegistrations
                    .AsNoTracking()
                    .Where(x => x.TenantKey == "takinschool")
                    .Include(x => x.User!)
                        .ThenInclude(x => x.Address!)
                            .ThenInclude(x => x.City)
                    .Include(x => x.StudentDetails)
                    .Include(x => x.FatherParent)
                    .Include(x => x.MotherParent)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(x =>
                        x.CurrentSchoolName.Contains(searchText) ||
                        x.TargetGrade.Contains(searchText) ||
                        x.RegistrationStatus.Contains(searchText) ||
                        x.User!.FirstName.Contains(searchText) ||
                        x.User.LastName.Contains(searchText) ||
                        x.User.NationalCode.Contains(searchText) ||
                        x.User.Username.Contains(searchText) ||
                        x.FatherParent!.Name.Contains(searchText) ||
                        x.FatherParent.ContactNumber.Contains(searchText) ||
                        x.MotherParent!.Name.Contains(searchText) ||
                        x.MotherParent.ContactNumber.Contains(searchText));
                }

                result.TotalCount = await query.CountAsync();
                result.PageCount = DbTools.GetPageCount(result.TotalCount, pageSize);

                var rows = await query
                    .OrderByDescending(x => x.ID)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var examRegistrations = await GetExamRegistrationLookupAsync(rows
                    .Select(x => x.User?.Username)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x!)
                    .ToList());

                result.Results = rows.Select(row => ToVm(row, examRegistrations)).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("GetTakinSchoolRegistrationById")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolRegistrationVM>>> GetTakinSchoolRegistrationById(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new RowResultObject<TakinSchoolRegistrationVM>();

            try
            {
                var row = await _schoolDb.SchoolRegistrations
                    .AsNoTracking()
                    .Where(x => x.ID == requestBody.ID && x.TenantKey == "takinschool")
                    .Include(x => x.User!)
                        .ThenInclude(x => x.Address!)
                            .ThenInclude(x => x.City)
                    .Include(x => x.StudentDetails)
                    .Include(x => x.FatherParent)
                    .Include(x => x.MotherParent)
                    .SingleOrDefaultAsync();

                if (row == null)
                {
                    result.Status = false;
                    result.ErrorMessage = "ثبت‌نام مدرسه پیدا نشد";
                    return BadRequest(result);
                }

                var examRegistrations = await GetExamRegistrationLookupAsync(new List<string> { row.User?.Username ?? "" });
                result.Result = ToVm(row, examRegistrations);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("AddTakinSchoolRegistration")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolRegistrationVM>>> AddTakinSchoolRegistration(EditTakinSchoolRegistrationRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new RowResultObject<TakinSchoolRegistrationVM>();
            var phoneNumber = OnlyDigits(requestBody.PhoneNumber);
            var nationalCode = OnlyDigits(requestBody.NationalCode);
            var fatherPhone = OnlyDigits(requestBody.FatherPhone);
            var motherPhone = OnlyDigits(requestBody.MotherPhone);
            var postalCode = OnlyDigits(requestBody.AddressPostalCode);

            var validationError = ValidateRegistrationPayload(phoneNumber, nationalCode, fatherPhone, motherPhone, postalCode);
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                result.Status = false;
                result.ErrorMessage = validationError;
                return BadRequest(result);
            }

            var email = string.IsNullOrWhiteSpace(requestBody.Email)
                ? $"takinschool-{nationalCode}@takinschool.local"
                : requestBody.Email.Trim();

            var duplicatePhone = await _schoolDb.Users.AsNoTracking().AnyAsync(x => x.Username == phoneNumber);
            if (duplicatePhone)
            {
                result.Status = false;
                result.ErrorMessage = "این شماره موبایل قبلا در سیستم ثبت شده است";
                return BadRequest(result);
            }

            var duplicateEmail = await _schoolDb.Users.AsNoTracking().AnyAsync(x => x.Email == email);
            if (duplicateEmail)
            {
                result.Status = false;
                result.ErrorMessage = "پست الکترونیک تکراری است";
                return BadRequest(result);
            }

            var duplicateNationalCode = await _schoolDb.Users.AsNoTracking().AnyAsync(x => x.NationalCode == nationalCode);
            if (duplicateNationalCode)
            {
                result.Status = false;
                result.ErrorMessage = "کد ملی تکراری است";
                return BadRequest(result);
            }

            var cityId = requestBody.CityId > 0 ? requestBody.CityId : await GetDefaultCityIdAsync();
            if (cityId <= 0)
            {
                result.Status = false;
                result.ErrorMessage = "شهر پیش‌فرض برای ثبت آدرس پیدا نشد";
                return BadRequest(result);
            }

            await using var transaction = await _schoolDb.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;
                var address = new Address
                {
                    CreateDate = now,
                    UpdateDate = now,
                    IsActive = true,
                    CityID = cityId,
                    AddressStreet = (requestBody.AddressStreet ?? "").Trim(),
                    AddressPostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode,
                    AddressLocationHorizentalPoint = "",
                    AddressLocationVerticalPoint = "",
                };

                var studentDetails = new StudentDetails
                {
                    CreateDate = now,
                    UpdateDate = now,
                    IsActive = true,
                };

                var user = new User
                {
                    CreateDate = now,
                    UpdateDate = now,
                    IsActive = true,
                    FirstName = (requestBody.FirstName ?? "").Trim(),
                    LastName = (requestBody.LastName ?? "").Trim(),
                    Username = phoneNumber,
                    Email = email,
                    NationalCode = nationalCode,
                    PasswordHash = $"Takin@{Guid.NewGuid():N}".ToHash(),
                    RoleId = (long)BaseRole.Student,
                    Address = address,
                    StudentDetails = studentDetails,
                    PermissionsVersion = 1,
                };

                var fatherParent = CreateParent(requestBody.FatherName, fatherPhone, requestBody.FatherJob, requestBody.FatherEducation, now);
                var motherParent = CreateParent(requestBody.MotherName, motherPhone, requestBody.MotherJob, requestBody.MotherEducation, now);
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
                    IsActive = true,
                    UserId = user.ID,
                    StudentDetailsId = studentDetails.ID,
                    FatherParentId = fatherParent.ID,
                    MotherParentId = motherParent.ID,
                    TenantKey = "takinschool",
                    FormType = "elementary-school-registration",
                    CurrentSchoolName = (requestBody.CurrentSchoolName ?? "").Trim(),
                    TargetGrade = (requestBody.TargetGrade ?? "").Trim(),
                    SeatNumber = ParseSeatNumber(requestBody.SeatNumber),
                    RegistrationStatus = string.IsNullOrWhiteSpace(requestBody.RegistrationStatus)
                        ? "Submitted"
                        : requestBody.RegistrationStatus.Trim(),
                };

                await _schoolDb.SchoolRegistrations.AddAsync(schoolRegistration);
                await _schoolDb.SaveChangesAsync();
                await transaction.CommitAsync();

                var row = await LoadSchoolRegistrationAsync(schoolRegistration.ID);
                result.Result = ToVm(row!, await GetExamRegistrationLookupAsync(new List<string> { phoneNumber }));
                return Ok(result);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("EditTakinSchoolRegistration")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolRegistrationVM>>> EditTakinSchoolRegistration(EditTakinSchoolRegistrationRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new RowResultObject<TakinSchoolRegistrationVM>();
            var phoneNumber = OnlyDigits(requestBody.PhoneNumber);
            var nationalCode = OnlyDigits(requestBody.NationalCode);
            var fatherPhone = OnlyDigits(requestBody.FatherPhone);
            var motherPhone = OnlyDigits(requestBody.MotherPhone);
            var postalCode = OnlyDigits(requestBody.AddressPostalCode);

            var validationError = ValidateRegistrationPayload(phoneNumber, nationalCode, fatherPhone, motherPhone, postalCode);
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                result.Status = false;
                result.ErrorMessage = validationError;
                return BadRequest(result);
            }

            try
            {
                var row = await _schoolDb.SchoolRegistrations
                    .Where(x => x.ID == requestBody.Id && x.TenantKey == "takinschool")
                    .Include(x => x.User!)
                        .ThenInclude(x => x.Address)
                    .Include(x => x.StudentDetails)
                    .Include(x => x.FatherParent)
                    .Include(x => x.MotherParent)
                    .SingleOrDefaultAsync();

                if (row == null)
                {
                    result.Status = false;
                    result.ErrorMessage = "ثبت‌نام مدرسه پیدا نشد";
                    return BadRequest(result);
                }

                var duplicatePhone = await _schoolDb.Users
                    .AsNoTracking()
                    .AnyAsync(x => x.ID != row.UserId && x.Username == phoneNumber);

                if (duplicatePhone)
                {
                    result.Status = false;
                    result.ErrorMessage = "شماره موبایل برای کاربر دیگری ثبت شده است";
                    return BadRequest(result);
                }

                var duplicateNationalCode = await _schoolDb.Users
                    .AsNoTracking()
                    .AnyAsync(x => x.ID != row.UserId && x.NationalCode == nationalCode);

                if (duplicateNationalCode)
                {
                    result.Status = false;
                    result.ErrorMessage = "کد ملی برای کاربر دیگری ثبت شده است";
                    return BadRequest(result);
                }

                var now = DateTime.Now;
                row.CurrentSchoolName = (requestBody.CurrentSchoolName ?? "").Trim();
                row.TargetGrade = (requestBody.TargetGrade ?? "").Trim();
                row.RegistrationStatus = string.IsNullOrWhiteSpace(requestBody.RegistrationStatus)
                    ? row.RegistrationStatus
                    : requestBody.RegistrationStatus.Trim();
                row.SeatNumber = ParseSeatNumber(requestBody.SeatNumber);
                row.UpdateDate = now;

                if (row.User != null)
                {
                    row.User.FirstName = (requestBody.FirstName ?? "").Trim();
                    row.User.LastName = (requestBody.LastName ?? "").Trim();
                    row.User.NationalCode = nationalCode;
                    row.User.Username = phoneNumber;
                    row.User.Email = string.IsNullOrWhiteSpace(requestBody.Email)
                        ? $"takinschool-{nationalCode}@takinschool.local"
                        : requestBody.Email.Trim();
                    row.User.UpdateDate = now;

                    if (row.User.Address == null)
                    {
                        row.User.Address = new AITechDATA.Domain.Address
                        {
                            CreateDate = now,
                            IsActive = true,
                        };
                    }

                    row.User.Address.CityID = requestBody.CityId > 0 ? requestBody.CityId : 30;
                    row.User.Address.AddressStreet = (requestBody.AddressStreet ?? "").Trim();
                    row.User.Address.AddressPostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode;
                    row.User.Address.UpdateDate = now;
                }

                if (row.FatherParent != null)
                {
                    row.FatherParent.Name = (requestBody.FatherName ?? "").Trim();
                    row.FatherParent.ContactNumber = fatherPhone;
                    row.FatherParent.Job = (requestBody.FatherJob ?? "").Trim();
                    row.FatherParent.Education = (requestBody.FatherEducation ?? "").Trim();
                    row.FatherParent.UpdateDate = now;
                }

                if (row.MotherParent != null)
                {
                    row.MotherParent.Name = (requestBody.MotherName ?? "").Trim();
                    row.MotherParent.ContactNumber = motherPhone;
                    row.MotherParent.Job = (requestBody.MotherJob ?? "").Trim();
                    row.MotherParent.Education = (requestBody.MotherEducation ?? "").Trim();
                    row.MotherParent.UpdateDate = now;
                }

                await _schoolDb.SaveChangesAsync();

                var examRegistrations = await GetExamRegistrationLookupAsync(new List<string> { row.User?.Username ?? "" });
                result.Result = ToVm(row, examRegistrations);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        private async Task<Dictionary<string, AITechDATA.Domain.PreRegistration>> GetExamRegistrationLookupAsync(List<string> phoneNumbers)
        {
            var normalizedPhones = phoneNumbers
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(OnlyDigits)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (normalizedPhones.Count == 0)
            {
                return new Dictionary<string, AITechDATA.Domain.PreRegistration>();
            }

            var examRows = await _publicDb.PreRegistrations
                .AsNoTracking()
                .Where(x => x.EntityType == "TakinSchoolExam" && normalizedPhones.Contains(x.PhoneNumber))
                .OrderByDescending(x => x.ID)
                .ToListAsync();

            return examRows
                .GroupBy(x => OnlyDigits(x.PhoneNumber))
                .ToDictionary(x => x.Key, x => x.First());
        }

        private static string OnlyDigits(string? value)
        {
            return new string((value ?? "").Where(char.IsDigit).ToArray());
        }

        private async Task<long> GetDefaultCityIdAsync()
        {
            var defaultCity = await _schoolDb.Cities
                .AsNoTracking()
                .OrderByDescending(x => x.DefaultCity)
                .ThenBy(x => x.ID)
                .FirstOrDefaultAsync();

            return defaultCity?.ID ?? 0;
        }

        private async Task<AITechDATA.Domain.SchoolRegistration?> LoadSchoolRegistrationAsync(long id)
        {
            return await _schoolDb.SchoolRegistrations
                .AsNoTracking()
                .Where(x => x.ID == id && x.TenantKey == "takinschool")
                .Include(x => x.User!)
                    .ThenInclude(x => x.Address!)
                        .ThenInclude(x => x.City)
                .Include(x => x.StudentDetails)
                .Include(x => x.FatherParent)
                .Include(x => x.MotherParent)
                .SingleOrDefaultAsync();
        }

        private static string ValidateRegistrationPayload(string phoneNumber, string nationalCode, string fatherPhone, string motherPhone, string postalCode)
        {
            if (phoneNumber.Length != 11 || fatherPhone.Length != 11 || motherPhone.Length != 11)
            {
                return "شماره‌های تماس باید ۱۱ رقم باشند";
            }

            if (nationalCode.Length != 10)
            {
                return "کد ملی باید ۱۰ رقم باشد";
            }

            if (!string.IsNullOrWhiteSpace(postalCode) && postalCode.Length != 10)
            {
                return "کد پستی باید ۱۰ رقم باشد";
            }

            return "";
        }

        private static int? ParseSeatNumber(string? value)
        {
            var normalized = OnlyDigits(value);
            return int.TryParse(normalized, out var seatNumber) && seatNumber > 0 ? seatNumber : null;
        }

        private static Parent CreateParent(string? name, string phone, string? job, string? education, DateTime now)
        {
            return new Parent
            {
                CreateDate = now,
                UpdateDate = now,
                IsActive = true,
                Name = (name ?? "").Trim(),
                ContactNumber = OnlyDigits(phone),
                Job = string.IsNullOrWhiteSpace(job) ? null : job.Trim(),
                Education = string.IsNullOrWhiteSpace(education) ? null : education.Trim(),
            };
        }

        private static TakinSchoolRegistrationVM ToVm(
            AITechDATA.Domain.SchoolRegistration row,
            Dictionary<string, AITechDATA.Domain.PreRegistration>? examRegistrations = null)
        {
            var user = row.User;
            var address = user?.Address;
            var father = row.FatherParent;
            var mother = row.MotherParent;
            var examRegistration = examRegistrations?.GetValueOrDefault(OnlyDigits(user?.Username));

            return new TakinSchoolRegistrationVM
            {
                Id = row.ID,
                CreateDate = row.CreateDate,
                UpdateDate = row.UpdateDate,
                IsActive = row.IsActive,
                UserId = row.UserId,
                StudentDetailsId = row.StudentDetailsId,
                FatherParentId = row.FatherParentId,
                MotherParentId = row.MotherParentId,
                TenantKey = row.TenantKey,
                FormType = row.FormType,
                CurrentSchoolName = row.CurrentSchoolName,
                TargetGrade = row.TargetGrade,
                RegistrationStatus = row.RegistrationStatus,
                SeatNumber = row.SeatNumber?.ToString() ?? "",
                FirstName = user?.FirstName ?? "",
                LastName = user?.LastName ?? "",
                FullName = $"{user?.FirstName} {user?.LastName}".Trim(),
                NationalCode = user?.NationalCode ?? "",
                PhoneNumber = user?.Username ?? "",
                Email = user?.Email ?? "",
                IdentificationCode = user?.IdentificationCode ?? "",
                AddressId = user?.AddressId,
                CityId = address?.CityID,
                CityName = address?.City?.CityName ?? "",
                AddressStreet = address?.AddressStreet ?? "",
                AddressPostalCode = address?.AddressPostalCode ?? "",
                FatherName = father?.Name ?? "",
                FatherPhone = father?.ContactNumber ?? "",
                FatherJob = father?.Job ?? "",
                FatherEducation = father?.Education ?? "",
                MotherName = mother?.Name ?? "",
                MotherPhone = mother?.ContactNumber ?? "",
                MotherJob = mother?.Job ?? "",
                MotherEducation = mother?.Education ?? "",
                HasExamRegistration = examRegistration != null,
                ExamRegistrationId = examRegistration?.ID,
                ExamRegistrationDate = examRegistration?.RegistrationDate
            };
        }
    }

    public class TakinSchoolRegistrationVM
    {
        public long Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public long UserId { get; set; }
        public long StudentDetailsId { get; set; }
        public long FatherParentId { get; set; }
        public long MotherParentId { get; set; }
        public string TenantKey { get; set; } = "";
        public string FormType { get; set; } = "";
        public string CurrentSchoolName { get; set; } = "";
        public string TargetGrade { get; set; } = "";
        public string RegistrationStatus { get; set; } = "";
        public string SeatNumber { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string NationalCode { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string IdentificationCode { get; set; } = "";
        public long? AddressId { get; set; }
        public long? CityId { get; set; }
        public string CityName { get; set; } = "";
        public string AddressStreet { get; set; } = "";
        public string AddressPostalCode { get; set; } = "";
        public string FatherName { get; set; } = "";
        public string FatherPhone { get; set; } = "";
        public string FatherJob { get; set; } = "";
        public string FatherEducation { get; set; } = "";
        public string MotherName { get; set; } = "";
        public string MotherPhone { get; set; } = "";
        public string MotherJob { get; set; } = "";
        public string MotherEducation { get; set; } = "";
        public bool HasExamRegistration { get; set; }
        public long? ExamRegistrationId { get; set; }
        public DateTime? ExamRegistrationDate { get; set; }
    }

    public class EditTakinSchoolRegistrationRequestBody
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string NationalCode { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string? Email { get; set; }
        public string CurrentSchoolName { get; set; } = "";
        public string TargetGrade { get; set; } = "";
        public string RegistrationStatus { get; set; } = "";
        public string? SeatNumber { get; set; }
        public long CityId { get; set; } = 30;
        public string AddressStreet { get; set; } = "";
        public string? AddressPostalCode { get; set; }
        public string FatherName { get; set; } = "";
        public string FatherPhone { get; set; } = "";
        public string? FatherJob { get; set; }
        public string? FatherEducation { get; set; }
        public string MotherName { get; set; } = "";
        public string MotherPhone { get; set; } = "";
        public string? MotherJob { get; set; }
        public string? MotherEducation { get; set; }
    }
}
