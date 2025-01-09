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

namespace AITechDATA.DataLayer.Servisces
{
    public class ImageRep : IImageRep
    {
        private AiITechContext _context;

        public ImageRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddImageAsync(Image image)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Images.AddAsync(image);
                await _context.SaveChangesAsync();
                result.ID = image.ID;
                _context.Entry(image).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditImageAsync(Image image)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Images.Update(image);
                await _context.SaveChangesAsync();
                result.ID = image.ID;
                _context.Entry(image).State = EntityState.Detached;
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

        public async Task<ListResultObject<Image>> GetAllImagesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "")
        {
            ListResultObject<Image> results = new ListResultObject<Image>();
            try
            {
                var query = _context.Images
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.FileName) && x.FileName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.EntityType) && x.EntityType.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
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

        public async Task<BitResultObject> RemoveImageAsync(Image image)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
                result.ID = image.ID;
                _context.Entry(image).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveImageAsync(long imageId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var image = await GetImageByIdAsync(imageId);
                result = await RemoveImageAsync(image.Result);
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