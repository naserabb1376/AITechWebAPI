using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace AITechDATA.DataLayer.Services
{
    public class SubmitFormRep : ISubmitFormRep
    {
        private AITechContext _context;

        public SubmitFormRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddSubmitFormAsync(SubmitForm SubmitForm)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.SubmitForms.AddAsync(SubmitForm);
                await _context.SaveChangesAsync();
                result.ID = SubmitForm.ID;
                _context.Entry(SubmitForm).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditSubmitFormAsync(SubmitForm SubmitForm)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.SubmitForms.Update(SubmitForm);
                await _context.SaveChangesAsync();
                result.ID = SubmitForm.ID;
                _context.Entry(SubmitForm).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistSubmitFormAsync(long SubmitFormId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.SubmitForms
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == SubmitFormId);
                result.ID = SubmitFormId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<SubmitForm>> GetAllSubmitFormsAsync(
      string entityName = "", long creatorId = 0,
      int pageIndex = 1, int pageSize = 20,
      string searchText = "", string sortQuery = "")
        {
            ListResultObject<SubmitForm> results = new ListResultObject<SubmitForm>();

            try
            {
                var query = _context.SubmitForms.AsNoTracking().AsQueryable();

                // شرط‌های دینامیک فقط در صورت معتبر بودن

                if (creatorId > 0)
                    query = query.Where(x => x.CreatorId == creatorId);


                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(x => x.EntityName == entityName);

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.FormKey) && x.FormKey.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderBy(x => x.CreateDate)
                    .SortBy(sortQuery)
                    .ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }


      

        public async Task<RowResultObject<SubmitForm>> GetSubmitFormByIdAsync(long SubmitFormId)
        {
            RowResultObject<SubmitForm> result = new RowResultObject<SubmitForm>();
            try
            {
                result.Result = await _context.SubmitForms
                    .AsNoTracking()
                    //.Include(x => x.Assignment)
                    .SingleOrDefaultAsync(x => x.ID == SubmitFormId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<RowResultObject<SubmitFormObjDto>> GetSubmitFormObjAsync(long SubmitFormId = 0, string FormKey = "")
        {
            {
                RowResultObject<SubmitFormObjDto> result = new RowResultObject<SubmitFormObjDto>();
                try
                {
                    var form = await _context.SubmitForms
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.IsActive && (x.ID == SubmitFormId || x.FormKey.ToLower() == FormKey.ToLower())) ?? new SubmitForm();

                    var fields = await _context.FieldInForms.Include(x=> x.FormField).AsNoTracking()
                        .Where(x=> x.FormId == form.ID)
                        .Select(f=> new FormFieldDto()
                        {
                            DisplayName = f.FormField.DisplayName,
                            FieldName = f.FormField.FieldName,
                        }).ToListAsync();

                    result.Result = new SubmitFormObjDto();
                    result.Result.Fields = fields;
                    result.Result.CreateDate =form.CreateDate;
                    result.Result.UpdateDate =form.UpdateDate;
                    result.Result.ID =form.ID;
                    result.Result.OtherLangs =form.OtherLangs;
                    result.Result.IsActive =form.IsActive;
                    result.Result.Description =form.Description;
                    result.Result.CreatorId =form.CreatorId;
                    result.Result.EntityName =form.EntityName;
                    result.Result.FormKey =form.FormKey;
                    result.Result.Title =form.Title;
                }
                catch (Exception ex)
                {
                    result.Status = false;
                    result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                }
                return result;
            }
        }

        public async Task<BitResultObject> RemoveSubmitFormAsync(SubmitForm SubmitForm)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.SubmitForms.Remove(SubmitForm);
                await _context.SaveChangesAsync();
                result.ID = SubmitForm.ID;
                _context.Entry(SubmitForm).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveSubmitFormAsync(long SubmitFormId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var SubmitForm = await GetSubmitFormByIdAsync(SubmitFormId);
                result = await RemoveSubmitFormAsync(SubmitForm.Result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }
    }
}