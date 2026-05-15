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
    public class SoftwareRep : ISoftwareRep
    {
        private AITechContext _context;

        public SoftwareRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddSoftwareAsync(Software Software)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Softwares.AddAsync(Software);
                await _context.SaveChangesAsync();
                result.ID = Software.ID;
                _context.Entry(Software).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditSoftwareAsync(Software Software)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Softwares.Update(Software);
                await _context.SaveChangesAsync();
                result.ID = Software.ID;
                _context.Entry(Software).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistSoftwareAsync(long SoftwareId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Softwares
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == SoftwareId);
                result.ID = SoftwareId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<SoftwareListCustomResponse<Software>> GetAllSoftwaresAsync(long categoryId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            SoftwareListCustomResponse<Software> results = new SoftwareListCustomResponse<Software>();
            try
            {
                var query = _context.Softwares
                                .AsNoTracking()
                                .Include(x => x.Category)
                                .AsQueryable();

                if (categoryId > 0)
                {
                    query = query.Where(x => x.CategoryId == categoryId);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Note) && x.Note.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.DownloadUrl) && x.DownloadUrl.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }
                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderByDescending(x => x.CreateDate)
                    .Select(a => new Software
                    {
                        ID = a.ID,
                        Title = a.Title,
                        CategoryId = a.CategoryId,
                        Category = a.Category,
                        Note = a.Note,
                        DownloadUrl = a.DownloadUrl,
                        CreateDate = a.CreateDate,
                        UpdateDate = a.UpdateDate,
                        Description = a.Description,
                        OtherLangs = a.OtherLangs,
                        IsActive = a.IsActive
                    })
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map images for each Software
                results.ResultImages = results.Results
                    .ToDictionary(
                        user => user,
                        user => _context.Images
                            .Where(img => img.ForeignKeyId == user.ID && img.EntityType == "Software")
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

        public async Task<SoftwareRowCustomResponse<Software>> GetSoftwareByIdAsync(long SoftwareId)
        {
            SoftwareRowCustomResponse<Software> result = new SoftwareRowCustomResponse<Software>();
            try
            {
                result.Result = await _context.Softwares
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .SingleOrDefaultAsync(x => x.ID == SoftwareId);

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<Software, List<Image>?>
            {
                { result.Result, await _context.Images
                    .Where(img => img.ForeignKeyId == SoftwareId && img.EntityType == "Software")
                    .ToListAsync() }
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

        public async Task<BitResultObject> RemoveSoftwareAsync(Software Software)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Images = _context.Images.Where(img => img.ForeignKeyId == Software.ID && img.EntityType == "Software");
                _context.Images.RemoveRange(Images);

                _context.Softwares.Remove(Software);
                await _context.SaveChangesAsync();
                result.ID = Software.ID;
                _context.Entry(Software).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveSoftwareAsync(long SoftwareId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Software = await GetSoftwareByIdAsync(SoftwareId);
                result = await RemoveSoftwareAsync(Software.Result);
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
