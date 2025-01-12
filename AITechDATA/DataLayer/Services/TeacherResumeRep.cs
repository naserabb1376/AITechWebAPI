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

namespace AITechDATA.DataLayer.Services
{
    public class TeacherResumeRep : ITeacherResumeRep
    {
        private AiITechContext _context;

        public TeacherResumeRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddTeacherResumeAsync(TeacherResume teacherResume)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.TeacherResumes.AddAsync(teacherResume);
                await _context.SaveChangesAsync();
                result.ID = teacherResume.ID;
                _context.Entry(teacherResume).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditTeacherResumeAsync(TeacherResume teacherResume)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TeacherResumes.Update(teacherResume);
                await _context.SaveChangesAsync();
                result.ID = teacherResume.ID;
                _context.Entry(teacherResume).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistTeacherResumeAsync(long teacherResumeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.TeacherResumes
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == teacherResumeId);
                result.ID = teacherResumeId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<TeacherResume>> GetAllTeacherResumesAsync(long UserId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<TeacherResume> results = new ListResultObject<TeacherResume>();
            try
            {
                var query = _context.TeacherResumes
                    .AsNoTracking()
                    .Where(x =>
                        (UserId > 0 && x.UserId == UserId) ||
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.DateAchieved)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<TeacherResume>> GetTeacherResumeByIdAsync(long teacherResumeId)
        {
            RowResultObject<TeacherResume> result = new RowResultObject<TeacherResume>();
            try
            {
                result.Result = await _context.TeacherResumes
                    .AsNoTracking()
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.ID == teacherResumeId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTeacherResumeAsync(TeacherResume teacherResume)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TeacherResumes.Remove(teacherResume);
                await _context.SaveChangesAsync();
                result.ID = teacherResume.ID;
                _context.Entry(teacherResume).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTeacherResumeAsync(long teacherResumeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var teacherResume = await GetTeacherResumeByIdAsync(teacherResumeId);
                result = await RemoveTeacherResumeAsync(teacherResume.Result);
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