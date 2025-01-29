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
        Task<ListResultObject<Image>> GetAllImagesAsync(string entityType="",long foreignKey=0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Image>> GetImageByIdAsync(long imageId);

        Task<BitResultObject> AddImagesAsync(List<Image> images);

        Task<BitResultObject> EditImagesAsync(List<Image> images);

        Task<BitResultObject> RemoveImagesAsync(List<Image> images);

        Task<BitResultObject> RemoveImagesAsync(List<long> imageIds);

        Task<BitResultObject> ExistImageAsync(long imageId);
    }
}