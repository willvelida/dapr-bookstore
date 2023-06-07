using Bookstore.Api.Common.Dtos;
using Bookstore.Api.Common.Models;

namespace Bookstore.Api.Services.Interfaces
{
    public interface IBookstoreService
    {
        Task<List<Book>> GetBooksByCategory(string category);
        Task<List<Book>> GetBooksByAuthor(string author);
        Task<Book?> GetBookById(string bookId);
        Task<string> CreateNewBook(BookDto bookDto);
        Task<bool> UpdateBook(string bookId, BookDto bookDto);
        Task<bool> DeleteBook(string bookId);
    }
}
