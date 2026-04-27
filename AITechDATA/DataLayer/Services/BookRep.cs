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
    public class BookRep : IBookRep
    {
        private AITechContext _context;

        public BookRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddBookAsync(Book Book)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Books.AddAsync(Book);
                await _context.SaveChangesAsync();
                result.ID = Book.ID;
                _context.Entry(Book).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditBookAsync(Book Book)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Books.Update(Book);
                await _context.SaveChangesAsync();
                result.ID = Book.ID;
                _context.Entry(Book).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistBookAsync(long BookId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Books
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == BookId);
                result.ID = BookId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BookListCustomResponse<Book>> GetAllBooksAsync(long categoryId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            BookListCustomResponse<Book> results = new BookListCustomResponse<Book>();
            try
            {
                var query = _context.Books
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
                        (!string.IsNullOrEmpty(x.AuthorName) && x.AuthorName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }
                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderByDescending(x => x.CreateDate)
                    .Select(a => new Book
                    {
                        ID = a.ID,
                        Title = a.Title,
                        CategoryId = a.CategoryId,
                        Category = a.Category,
                        Note = a.Note,
                        AuthorName = a.AuthorName,
                        CreateDate = a.CreateDate,
                        UpdateDate = a.UpdateDate,
                        Description = a.Description,
                        OtherLangs = a.OtherLangs
                    })
                    .ToListAsync();

                // Map images for each Book
                results.ResultImages = results.Results
                    .ToDictionary(
                        user => user,
                        user => _context.Images
                            .Where(img => img.ForeignKeyId == user.ID && img.EntityType == "Book")
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

        public async Task<BookRowCustomResponse<Book>> GetBookByIdAsync(long BookId)
        {
            BookRowCustomResponse<Book> result = new BookRowCustomResponse<Book>();
            try
            {
                result.Result = await _context.Books
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .SingleOrDefaultAsync(x => x.ID == BookId);

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<Book, List<Image>?>
            {
                { result.Result, await _context.Images
                    .Where(img => img.ForeignKeyId == BookId && img.EntityType == "Book")
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

        public async Task<BitResultObject> RemoveBookAsync(Book Book)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Images = _context.Images.Where(img => img.ForeignKeyId == Book.ID && img.EntityType == "Book");
                _context.Images.RemoveRange(Images);

                _context.Books.Remove(Book);
                await _context.SaveChangesAsync();
                result.ID = Book.ID;
                _context.Entry(Book).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveBookAsync(long BookId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Book = await GetBookByIdAsync(BookId);
                result = await RemoveBookAsync(Book.Result);
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