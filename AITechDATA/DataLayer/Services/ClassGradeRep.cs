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
    public class ClassGradeRep : IClassGradeRep
    {
        private AITechContext _context;

        public ClassGradeRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddClassGradeAsync(ClassGrade ClassGrade)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.ClassGrades.AddAsync(ClassGrade);
                await _context.SaveChangesAsync();
                result.ID = ClassGrade.ID;
                _context.Entry(ClassGrade).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditClassGradeAsync(ClassGrade ClassGrade)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.ClassGrades.Update(ClassGrade);
                await _context.SaveChangesAsync();
                result.ID = ClassGrade.ID;
                _context.Entry(ClassGrade).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistClassGradeAsync(long ClassGradeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.ClassGrades
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == ClassGradeId);
                result.ID = ClassGradeId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<ClassGrade>> GetAllClassGradesAsync(
      string entityName = "", long ForeignKeyId = 0,
      int pageIndex = 1, int pageSize = 20,
      string searchText = "", string sortQuery = "")
        {
            ListResultObject<ClassGrade> results = new ListResultObject<ClassGrade>();

            try
            {
                var query = _context.ClassGrades.AsNoTracking().AsQueryable();

                // شرط‌های دینامیک فقط در صورت معتبر بودن
                if (ForeignKeyId > 0)
                    query = query.Where(x => x.ForeignKeyId == ForeignKeyId);


                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(x => x.EntityName == entityName);

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.GradeScore.ToString()) && x.GradeScore.ToString().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderByDescending(x => x.CreateDate)
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


      

        public async Task<RowResultObject<ClassGrade>> GetClassGradeByIdAsync(long ClassGradeId)
        {
            RowResultObject<ClassGrade> result = new RowResultObject<ClassGrade>();
            try
            {
                result.Result = await _context.ClassGrades
                    .AsNoTracking()
                    //.Include(x => x.Assignment)
                    .SingleOrDefaultAsync(x => x.ID == ClassGradeId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }


        public async Task<BitResultObject> RemoveClassGradeAsync(ClassGrade ClassGrade)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.ClassGrades.Remove(ClassGrade);
                await _context.SaveChangesAsync();
                result.ID = ClassGrade.ID;
                _context.Entry(ClassGrade).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveClassGradeAsync(long ClassGradeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var ClassGrade = await GetClassGradeByIdAsync(ClassGradeId);
                result = await RemoveClassGradeAsync(ClassGrade.Result);
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