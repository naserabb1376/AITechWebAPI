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

namespace AITechDATA.DataLayer.Servisces
{
    public class CourseRep : ICourseRep
    {
        private AiITechContext _context;

        public CourseRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddCourseAsync(Course course)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Courses.AddAsync(course);
                await _context.SaveChangesAsync();
                result.ID = course.ID;
                _context.Entry(course).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditCourseAsync(Course course)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                result.ID = course.ID;
                _context.Entry(course).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistCourseAsync(long courseId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Courses
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == courseId);
                result.ID = courseId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Course>> GetAllCoursesAsync(long categoryId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<Course> results = new ListResultObject<Course>();
            try
            {
                var query = _context.Courses
                    .AsNoTracking()
                    .Where(x =>
                        (categoryId > 0 && x.CategoryId == categoryId)
                        ||((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Category)
                    .Include(x => x.Groups)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Course>> GetCourseByIdAsync(long courseId)
        {
            RowResultObject<Course> result = new RowResultObject<Course>();
            try
            {
                result.Result = await _context.Courses
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Groups)
                    .SingleOrDefaultAsync(x => x.ID == courseId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveCourseAsync(Course course)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                result.ID = course.ID;
                _context.Entry(course).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveCourseAsync(long courseId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var course = await GetCourseByIdAsync(courseId);
                result = await RemoveCourseAsync(course.Result);
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