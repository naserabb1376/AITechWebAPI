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
    public class FormFieldRep : IFormFieldRep
    {
        private AITechContext _context;

        public FormFieldRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddFormFieldAsync(FormField FormField)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.FormFields.AddAsync(FormField);
                await _context.SaveChangesAsync();
                result.ID = FormField.ID;
                _context.Entry(FormField).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditFormFieldAsync(FormField FormField)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.FormFields.Update(FormField);
                await _context.SaveChangesAsync();
                result.ID = FormField.ID;
                _context.Entry(FormField).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistFormFieldAsync(long FormFieldId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.FormFields
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == FormFieldId);
                result.ID = FormFieldId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<FormField>> GetAllFormFieldsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<FormField> results = new ListResultObject<FormField>();

            try
            {
                var query = _context.FormFields.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.FieldName) && x.FieldName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.DisplayName) && x.DisplayName.Contains(searchText)) ||
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


      

        public async Task<RowResultObject<FormField>> GetFormFieldByIdAsync(long FormFieldId)
        {
            RowResultObject<FormField> result = new RowResultObject<FormField>();
            try
            {
                result.Result = await _context.FormFields
                    .AsNoTracking()
                    //.Include(x => x.Assignment)
                    .SingleOrDefaultAsync(x => x.ID == FormFieldId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }


        public async Task<BitResultObject> RemoveFormFieldAsync(FormField FormField)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.FormFields.Remove(FormField);
                await _context.SaveChangesAsync();
                result.ID = FormField.ID;
                _context.Entry(FormField).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveFormFieldAsync(long FormFieldId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var FormField = await GetFormFieldByIdAsync(FormFieldId);
                result = await RemoveFormFieldAsync(FormField.Result);
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