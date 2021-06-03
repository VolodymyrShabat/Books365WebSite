
using Books365WebSite.Infrustructure;
using Books365WebSite.Interfaces;
using Books365WebSite.Models;
using Books365WebSite.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class Repository : IRepository
{
    private readonly Context _context;
    private readonly UserManager<User> _userManager;

    public Repository(Context context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<List<Book>> GetAllBooks() => await _context.Books.ToListAsync();
    public async Task<Book> GetBookById(int? id) => await _context.Books.FindAsync(id);
    public async Task<User> GetCurrentUserAsync(ClaimsPrincipal claims) => await _userManager.GetUserAsync(claims);
    public async Task<ReadingStatus> GetBookReadingStatus(int? id, ClaimsPrincipal claims)
    {
        var currentUser = await GetCurrentUserAsync(claims);
        var userId = currentUser?.Id;

        return await _context.ReadingStatuses.FirstOrDefaultAsync(x => x.BookId == id && x.UserId == userId);
    }

    public async Task DeleteReadingStatus(int id, ClaimsPrincipal claims)
    {
        var readingStatus = await GetBookReadingStatus(id, claims);

        _context.ReadingStatuses.Remove(readingStatus);
        await _context.SaveChangesAsync();
    }

    public Task DeleteBook(int id)
    {
        throw new System.NotImplementedException();
    }

    public async Task UpdateBook(Book book)
    {
        Book bookfromDB = await GetBookById(book.Isbn);
        await _context.AddAsync(book);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateReadingStatus(CreatingViewModel model, ClaimsPrincipal claims)
    {
        var currentUser = await GetCurrentUserAsync(claims);
        var userId = currentUser?.Id;

        Book bookfromDB = await GetBookById(model.Book.Isbn);
        ReadingStatus status = (await GetBookReadingStatus(model.Book.Isbn, claims));

        if (bookfromDB is null)
            await _context.AddAsync(model.Book);
        else
        {
            status.PagesRead = model.Status.PagesRead;
            status.Status = model.Status.Status;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<Statistic> GetStatistic(ClaimsPrincipal claims)
    {
        var currentUser = await GetCurrentUserAsync(claims);
        var userId = currentUser?.Id;
        var booksIdOfCurrentUser = _context.ReadingStatuses.Where(x => x.UserId == userId);

        var pagesReadOfCurrentUserList = _context.ReadingStatuses.Where(x => x.UserId == userId);
        var allReadPages = pagesReadOfCurrentUserList.Sum(x => x.PagesRead);

        var booksOfCurrentUser = _context.Books.Where(x => booksIdOfCurrentUser.Any(b => x.Isbn == b.BookId)).ToList();
        var booksReadOfCurrentUser = _context.Books.Where(x => booksIdOfCurrentUser.Any(b => x.Isbn == b.BookId && b.Status == "Read")).ToList();

        var booksInProgressOfCurrentUser = _context.Books.Where(x => booksIdOfCurrentUser.Any(b => x.Isbn == b.BookId && b.Status == "In progress")).ToList();

        var favouriteAuthor = booksOfCurrentUser.GroupBy(s => s.Author)
                     .OrderByDescending(s => s.Count()).FirstOrDefault();
        string favouriteAuthorString = string.Empty;
        if (favouriteAuthor != null)
        {
            favouriteAuthorString = favouriteAuthor.Key;
        }
        var firstBookId = _context.ReadingStatuses.Where(x => x.UserId == userId).OrderBy(x => x.DateStarted).Select(x => x.BookId).FirstOrDefault();

        var firstBook = _context.Books.Where(x => x.Isbn == firstBookId).Select(x => x.Title).FirstOrDefault();

    Statistic statistic = new Statistic()
    {
        AmountOfUserBooks = booksOfCurrentUser.Count,
        BooksRead = booksReadOfCurrentUser.Count,
        BooksInProgress = booksInProgressOfCurrentUser.Count,
        PagesRead = allReadPages,
        FavouriteAuthor = favouriteAuthorString,
        FirstBook = firstBook,
    };

        return statistic;

    }

    public async Task<List<ReadingStatusViewModel>> GetUserBooks(ClaimsPrincipal claims)
    {
        var currentUser = await GetCurrentUserAsync(claims);

        var userId = currentUser?.Id;
        var statusesOfUser = _context.ReadingStatuses.Where(x => x.UserId == userId);

        var books = _context.Books.Where(x => statusesOfUser.Any(b => x.Isbn == b.BookId)).ToList();

        var result = from book in books
                     join statuses in statusesOfUser on book.Isbn equals statuses.BookId
                     select new { Isbn = book.Isbn, Read = statuses.PagesRead, Date = statuses.DateStarted, Author = book.Author, Title = book.Title, Pages = book.Pages, Genre = book.Genre, Status = statuses.Status };


        List<ReadingStatusViewModel> data = new();
        foreach (var item in result)
        {
            Book book = new() { Isbn = item.Isbn, Genre = item.Genre, Author = item.Author, Pages = item.Pages, Title = item.Title };
            ReadingStatus statuses = new() { BookId = item.Isbn, Status = item.Status, PagesRead = item.Read, DateStarted = item.Date };

            data.Add(new() { Book = book, Status = statuses });
        }

        return data;
    }

    public async Task<ReadingStatus> GetBookStatus(int id, ClaimsPrincipal claims)
    {
        var currentUser = await GetCurrentUserAsync(claims);
        var userId = currentUser?.Id;

        return await _context.ReadingStatuses.FirstOrDefaultAsync(u => u.BookId == id && u.UserId == userId);

    }

    public async Task AddBook(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }
    public async Task AddReadingStatus(ReadingStatus status)
    {
        await _context.ReadingStatuses.AddAsync(status);
        await _context.SaveChangesAsync();
    }

}