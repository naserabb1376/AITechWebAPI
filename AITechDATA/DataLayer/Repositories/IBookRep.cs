using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IBookRep
    {
        Task<BookListCustomResponse<Book>> GetAllBooksAsync(long categoryId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<BookRowCustomResponse<Book>> GetBookByIdAsync(long BookId);

        Task<BitResultObject> AddBookAsync(Book Book);

        Task<BitResultObject> EditBookAsync(Book Book);

        Task<BitResultObject> RemoveBookAsync(Book Book);

        Task<BitResultObject> RemoveBookAsync(long BookId);

        Task<BitResultObject> ExistBookAsync(long BookId);
    }
}