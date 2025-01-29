using AITechDATA.DataLayer.Repositories;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AITechDATA.DataLayer.Services
{
    public class ImageRep : IImageRep
    {
        private AiITechContext _context;

        public ImageRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddImagesAsync(List<Image> images)
        {
            BitResultObject result = new BitResultObject();
            try
            {
               await _context.Images.AddRangeAsync(images);
                await _context.SaveChangesAsync();
                result.ID = images.FirstOrDefault().ID;
                foreach (var image in images)
                {
                    _context.Entry(image).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditImagesAsync(List<Image> images)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Images.UpdateRange(images);
                await _context.SaveChangesAsync();
                result.ID = images.FirstOrDefault().ID;
                foreach (var image in images)
                {
                    _context.Entry(image).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistImageAsync(long imageId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Images
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == imageId);
                result.ID = imageId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Image>> GetAllImagesAsync(string entityType = "", long foreignKey = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<Image> results = new ListResultObject<Image>();
            try
            {
                var query = _context.Images
                    .AsNoTracking()
                    .Where(x =>
                        (
                    (foreignKey > 0 && x.ForeignKeyId == foreignKey) ||
                    (!string.IsNullOrEmpty(entityType) && x.EntityType == entityType)
                        ) ||
                       ((!string.IsNullOrEmpty(x.FileName) && x.FileName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.EntityType) && x.EntityType.Contains(searchText))
                    ));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
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

        public async Task<RowResultObject<Image>> GetImageByIdAsync(long imageId)
        {
            RowResultObject<Image> result = new RowResultObject<Image>();
            try
            {
                result.Result = await _context.Images
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == imageId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveImagesAsync(List<Image> images)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Images.RemoveRange(images);
                await _context.SaveChangesAsync();
                result.ID = images.FirstOrDefault().ID;
                foreach (var image in images)
                {
                    _context.Entry(image).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveImagesAsync(List<long> ImageIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var ImagesToRemove = new List<Image>();

                foreach (var ImageId in ImageIds)
                {
                    var Image = await GetImageByIdAsync(ImageId);
                    if (Image.Result != null)
                    {
                        ImagesToRemove.Add(Image.Result);
                    }
                }

                if (ImagesToRemove.Any())
                {
                    result = await RemoveImagesAsync(ImagesToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching Images found to remove.";
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