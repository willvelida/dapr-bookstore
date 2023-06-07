using Bookstore.Api.Common.Dtos;
using Bookstore.Api.Common.Models;
using Bookstore.Api.Services.Interfaces;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace Bookstore.Api.Services
{
    public class BookstoreService : IBookstoreService
    {
        private static string STORE_NAME = "statestore";
        private readonly DaprClient _daprClient;
        private readonly ILogger<BookstoreService> _logger;

        public BookstoreService(DaprClient daprClient, ILogger<BookstoreService> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public async Task<string> CreateNewBook(BookDto bookDto)
        {
            try
            {
                var book = new Book
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    Category = bookDto.Category,
                    Price = bookDto.Price
                };

                _logger.LogError($"Save a new book with name: {book.Title}");

                await _daprClient.SaveStateAsync<Book>(STORE_NAME, book.Id, book);
                return book.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateNewBook)}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteBook(string bookId)
        {
            try
            {
                _logger.LogInformation($"Delete book with ID: {bookId}");
                await _daprClient.DeleteStateAsync(STORE_NAME, bookId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(DeleteBook)}: {ex.Message}");
                throw;
            }
        }

        public async Task<Book?> GetBookById(string bookId)
        {
            try
            {
                _logger.LogInformation($"Getting Book with Id: {bookId}");
                return await _daprClient.GetStateAsync<Book>(STORE_NAME, bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(DeleteBook)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Book>> GetBooksByCategory(string category)
        {
            try
            {
                var query = "{" +
                "\"filter\": {" +
                        "\"EQ\": { \"category\": \"" + category + "\" }" +
                    "}}";

                var queryResponse = await _daprClient.QueryStateAsync<Book>(STORE_NAME, query);

                return queryResponse.Results.Select(q => q.Data).OrderByDescending(o => o.Category).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetBooksByCategory)}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateBook(string bookId, BookDto bookDto)
        {
            try
            {
                var existingBook = await _daprClient.GetStateAsync<Book>(STORE_NAME, bookId);

                if (existingBook is not null)
                {
                    existingBook.Title = bookDto.Title;
                    existingBook.Author = bookDto.Author;
                    existingBook.Price = bookDto.Price;
                    existingBook.Category = bookDto.Category;
                    await _daprClient.SaveStateAsync<Book>(STORE_NAME, bookId, existingBook);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(UpdateBook)}: {ex.Message}");
                throw;
            }
        }
    }
}
