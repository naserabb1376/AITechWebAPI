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
using System.Runtime.ConstrainedExecution;
using MTPermissionCenter.EFCore.Entities;

namespace AITechDATA.DataLayer.Services
{
    public class FieldInFormRep : IFieldInFormRep
    {
        private AITechContext _context;

        public FieldInFormRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddFieldInFormsAsync(List<FieldInForm> FieldInForms)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                FieldInForms = FieldInForms.Where(p=>  ! _context.FieldInForms.Any(x=> x.FormId == p.FormId && x.FormFieldId == p.FormFieldId)).ToList();
                await _context.FieldInForms.AddRangeAsync(FieldInForms);
                await _context.SaveChangesAsync();
                result.ID = FieldInForms.Count> 0 ?  FieldInForms.FirstOrDefault().ID : 0;
                foreach (var FieldInForm in FieldInForms)
                {
                    _context.Entry(FieldInForm).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditFieldInFormsAsync(List<FieldInForm> FieldInForms)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                FieldInForms = FieldInForms.Where(p => !_context.FieldInForms.Any(x => x.FormId == p.FormId && x.FormFieldId == p.FormFieldId)).ToList();
                _context.FieldInForms.UpdateRange(FieldInForms);
                await _context.SaveChangesAsync();
                result.ID = FieldInForms.Count > 0 ? FieldInForms.FirstOrDefault().ID : 0;
                foreach (var FieldInForm in FieldInForms)
                {
                    _context.Entry(FieldInForm).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistFieldInFormAsync(long FieldInFormId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.FieldInForms
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == FieldInFormId);
                result.ID = FieldInFormId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<FieldInForm>> GetAllFieldInFormsAsync(long FormId = 0, long FormFieldId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<FieldInForm> results = new ListResultObject<FieldInForm>();
            try
            {
                var query = _context.FieldInForms.Include(x => x.Form).Include(x=> x.FormField).AsNoTracking();
                if (FormId > 0)
                {
                    query = query.Where(x => x.FormId == FormId);
                }

                if (FormFieldId > 0)
                {
                    query = query.Where(x => x.FormFieldId == FormFieldId);

                }

                query = query.Where(x =>
                        x.Form.FormKey.ToString().Contains(searchText) ||
                        x.Form.Title.ToString().Contains(searchText) ||
                        x.Form.Description.ToString().Contains(searchText) ||
                        x.Form.EntityName.ToString().Contains(searchText) ||
                        x.FormField.FieldName.ToString().Contains(searchText) ||
                        x.FormField.DisplayName.ToString().Contains(searchText) ||
                        x.Form.Description.ToString().Contains(searchText)
                    );


                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.ID)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<FieldInForm>> GetFieldInFormByIdAsync(long FieldInFormId)
        {
            RowResultObject<FieldInForm> result = new RowResultObject<FieldInForm>();
            try
            {
                result.Result = await _context.FieldInForms
                    .AsNoTracking()
                    .Include(x => x.Form).Include(x=> x.FormField)
                    .SingleOrDefaultAsync(x => x.ID == FieldInFormId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveFieldInFormsAsync(List<FieldInForm> FieldInForms)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.FieldInForms.RemoveRange(FieldInForms);
                await _context.SaveChangesAsync();
                result.ID = FieldInForms.FirstOrDefault().ID;
                foreach (var FieldInForm in FieldInForms)
                {
                    _context.Entry(FieldInForm).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveFieldInFormsAsync(List<long> FieldInFormIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var FieldInFormsToRemove = new List<FieldInForm>();

                foreach (var FieldInFormId in FieldInFormIds)
                {
                    var FieldInForm = await GetFieldInFormByIdAsync(FieldInFormId);
                    if (FieldInForm.Result != null)
                    {
                        FieldInFormsToRemove.Add(FieldInForm.Result);
                    }
                }

                if (FieldInFormsToRemove.Any())
                {
                    result = await RemoveFieldInFormsAsync(FieldInFormsToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching FieldInForms found to remove.";
                }
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