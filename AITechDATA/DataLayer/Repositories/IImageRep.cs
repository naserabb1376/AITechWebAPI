using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IImageRep
    {
        Task<ListResultObject<Image>> GetAllImagesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Image>> GetImageByIdAsync(long imageId);

        Task<BitResultObject> AddImageAsync(Image image);

        Task<BitResultObject> EditImageAsync(Image image);

        Task<BitResultObject> RemoveImageAsync(Image image);

        Task<BitResultObject> RemoveImageAsync(long imageId);

        Task<BitResultObject> ExistImageAsync(long imageId);
    }
}