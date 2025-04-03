using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IFileUploadRep
    {
        Task<ListResultObject<FileUpload>> GetAllFileUploadsAsync(long assignmentId = 0,long creatorId=0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<FileUpload>> GetFileUploadByIdAsync(long fileUploadId);
        Task<RowResultObject<FileUpload>> GetFileForDownloadAsync(long fileUploadId=0,long foreignKeyId=0,long userId=0, long roleId = 0);

        Task<BitResultObject> AddFileUploadAsync(FileUpload fileUpload);

        Task<BitResultObject> EditFileUploadAsync(FileUpload fileUpload);

        Task<BitResultObject> RemoveFileUploadAsync(FileUpload fileUpload);

        Task<BitResultObject> RemoveFileUploadAsync(long fileUploadId);

        Task<BitResultObject> ExistFileUploadAsync(long fileUploadId);
    }
}