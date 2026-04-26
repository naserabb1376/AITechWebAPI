using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ICategoryRep
    {
        Task<ListResultObject<Category>> GetAllCategoriesAsync(string categoryEntityType ="",int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Category>> GetCategoryByIdAsync(long categoryId);

        Task<BitResultObject> AddCategoryAsync(Category category);

        Task<BitResultObject> EditCategoryAsync(Category category);

        Task<BitResultObject> RemoveCategoryAsync(Category category);

        Task<BitResultObject> RemoveCategoryAsync(long categoryId);

        Task<BitResultObject> ExistCategoryAsync(long categoryId);
    }
}