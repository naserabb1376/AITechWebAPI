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
using AITechDATA.CustomResponses;

namespace AITechDATA.DataLayer.Services
{
    public class CourseRep : ICourseRep
    {
        private AITechContext _context;

        public CourseRep(AITechContext context)
        {
            _context = context;
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

        public async Task<CourseListCustomResponse<CourseDto>> GetAllCoursesAsync(
      long categoryId = 0,
      int pageIndex = 1,
      int pageSize = 20,
      string searchText = "",
      string sortQuery = "")
        {
            var results = new CourseListCustomResponse<CourseDto>();

            try
            {
                var query = _context.Courses.Include(g=> g.Groups).ThenInclude(g=> g.Teacher).ThenInclude(g=> g.TeacherResume)
                    .AsNoTracking()
                    .Select(a => new CourseDto
                    {
                        ID = a.ID,
                        Title = a.Title,
                        Description = a.Description,
                        Note = a.Note,
                        CategoryId = a.CategoryId,
                        Category = a.Category,
                        CreateDate = a.CreateDate,
                        UpdateDate = a.UpdateDate,

                        Groups = a.Groups.Select(g => new CourseGroupDto
                        {
                            ID = g.ID,
                            Name = g.Name,
                            DayOfWeek = g.DayOfWeek,
                            StartDate = g.StartDate,
                            EndDate = g.EndDate,
                            StartTime = g.StartTime,
                            EndTime = g.EndTime,
                            Fee = g.Fee,
                            Status = g.Status,
                            GroupType = g.GroupType,
                            Note = g.Note,
                            TeacherId = g.TeacherId,

                            TeacherFirstName = g.Teacher.FirstName,
                            TeacherLastName = g.Teacher.LastName,
                            IsActive = a.IsActive,
                            TeacherCVLink = _context.FileUploads.Where(f => f.EntityType.ToLower() == "TeacherResume".ToLower() && f.ForeignKeyId == g.Teacher.TeacherResume.ID)
                            .Select(x => x.GetUrl).FirstOrDefault().CreateDownloadLink(),
                            CreateDate=a.CreateDate,
                            UpdateDate=a.UpdateDate,
                            OtherLangs = a.OtherLangs,
                            TeacherResumeId = g.Teacher.TeacherResume.ID,
                            TeacherResumeDateAchieved = g.Teacher.TeacherResume.DateAchieved,
                            TeacherResumeDescription = g.Teacher.TeacherResume.Description,
                            TeacherResumeTitle = g.Teacher.TeacherResume.Title,

                        }).ToList()
                    });

                if (categoryId > 0)
                {
                    query = query.Where(x => x.CategoryId == categoryId);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Note) && x.Note.Contains(searchText)) ||
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

                results.ResultImages = results.Results.ToDictionary(
                    course => course,
                    course => _context.Images
                        .Where(img => img.ForeignKeyId == course.ID && img.EntityType.ToLower() == "course")
                        .ToList()
                );
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }
        public async Task<CourseRowCustomResponse<CourseDto>> GetCourseByIdAsync(long courseId)
        {
            var result = new CourseRowCustomResponse<CourseDto>();

            try
            {
                result.Result = await _context.Courses.Include(g => g.Groups).ThenInclude(g => g.Teacher).ThenInclude(g => g.TeacherResume)
                    .AsNoTracking()
                    .Where(x => x.ID == courseId)
                    .Select(a => new CourseDto
                    {
                        ID = a.ID,
                        Title = a.Title,
                        Description = a.Description,
                        Note = a.Note,
                        CategoryId = a.CategoryId,
                        Category = a.Category,
                        CreateDate = a.CreateDate,
                        UpdateDate = a.UpdateDate,

                        Groups = a.Groups.Select(g => new CourseGroupDto
                        {
                            ID = g.ID,
                            Name = g.Name,
                            DayOfWeek = g.DayOfWeek,
                            StartDate = g.StartDate,
                            EndDate = g.EndDate,
                            StartTime = g.StartTime,
                            EndTime = g.EndTime,
                            Fee = g.Fee,
                            Status = g.Status,
                            GroupType = g.GroupType,
                            Note = g.Note,
                            TeacherId = g.TeacherId,

                            TeacherFirstName = g.Teacher.FirstName,
                            TeacherLastName = g.Teacher.LastName,
                            TeacherCVLink = _context.FileUploads.Where(f => f.EntityType.ToLower() == "TeacherResume".ToLower() && f.ForeignKeyId == g.Teacher.TeacherResume.ID)
                            .Select(x => x.GetUrl).FirstOrDefault().CreateDownloadLink(),
                            IsActive = a.IsActive,
                            CreateDate = a.CreateDate,
                            UpdateDate = a.UpdateDate,
                            OtherLangs = a.OtherLangs,
                            TeacherResumeId = g.Teacher.TeacherResume.ID,
                            TeacherResumeDateAchieved = g.Teacher.TeacherResume.DateAchieved,
                            TeacherResumeDescription = g.Teacher.TeacherResume.Description,
                            TeacherResumeTitle = g.Teacher.TeacherResume.Title,
                            

                        }).ToList()
                    })
                    .SingleOrDefaultAsync();

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<CourseDto, List<Image>?>
            {
                {
                    result.Result,
                    await _context.Images
                        .Where(img => img.ForeignKeyId == courseId && img.EntityType.ToLower() == "course")
                        .ToListAsync()
                }
            };
                }
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
                var Images = _context.Images.Where(img => img.ForeignKeyId == course.ID && img.EntityType == "Course");
                _context.Images.RemoveRange(Images);

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
                var courseDto = await GetCourseByIdAsync(courseId);
                var theCourse = new Course
                {
                    OtherLangs = courseDto.Result.OtherLangs,
                    CategoryId = courseDto.Result.CategoryId,
                    CreateDate = courseDto.Result.CreateDate,
                    IsActive = courseDto.Result.IsActive,
                    UpdateDate = courseDto.Result.UpdateDate,
                    Description = courseDto.Result.Description,
                    ID = courseDto.Result.ID,
                    Note = courseDto.Result.Note,
                    Title = courseDto.Result.Title,
                    
                };
                result = await RemoveCourseAsync(theCourse);
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