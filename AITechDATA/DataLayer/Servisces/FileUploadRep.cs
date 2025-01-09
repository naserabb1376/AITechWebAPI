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
    public class FileUploadRep : IFileUploadRep
    {
        private AiITechContext _context;

        public FileUploadRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddFileUploadAsync(FileUpload fileUpload)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.FileUploads.AddAsync(fileUpload);
                await _context.SaveChangesAsync();
                result.ID = fileUpload.ID;
                _context.Entry(fileUpload).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditFileUploadAsync(FileUpload fileUpload)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.FileUploads.Update(fileUpload);
                await _context.SaveChangesAsync();
                result.ID = fileUpload.ID;
                _context.Entry(fileUpload).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistFileUploadAsync(long fileUploadId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.FileUploads
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == fileUploadId);
                result.ID = fileUploadId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<FileUpload>> GetAllFileUploadsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "")
        {
            ListResultObject<FileUpload> results = new ListResultObject<FileUpload>();
            try
            {
                var query = _context.FileUploads
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.FileName) && x.FileName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.FilePath) && x.FilePath.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.ContentType) && x.ContentType.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                    .ToPaging(pageIndex, pageSize)
                    .Include(x => x.Assignment)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<FileUpload>> GetFileUploadByIdAsync(long fileUploadId)
        {
            RowResultObject<FileUpload> result = new RowResultObject<FileUpload>();
            try
            {
                result.Result = await _context.FileUploads
                    .AsNoTracking()
                    .Include(x => x.Assignment)
                    .SingleOrDefaultAsync(x => x.ID == fileUploadId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveFileUploadAsync(FileUpload fileUpload)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.FileUploads.Remove(fileUpload);
                await _context.SaveChangesAsync();
                result.ID = fileUpload.ID;
                _context.Entry(fileUpload).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveFileUploadAsync(long fileUploadId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var fileUpload = await GetFileUploadByIdAsync(fileUploadId);
                result = await RemoveFileUploadAsync(fileUpload.Result);
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