
using Books365WebSite.Models;
using Books365WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Books365WebSite.Controllers
{
    public class HomeController : Controller
    {

        private Context _db;
        private UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager, Context db)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetBooks() => View(/*await _db.Books.ToListAsync()*/);
        

        [HttpGet]
        public async Task<IActionResult> Books() => Json(new { data = await _db.Books.ToListAsync() });





        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserBooks()
        {
            var currentUser = await GetCurrentUserAsync();

            var userId = currentUser?.Id;
            var statusesOfUser = _db.ReadingStatuses.Where(x => x.UserId == userId);

            var books = _db.Books.Where(x => statusesOfUser.Any(b => x.Isbn == b.BookId)).ToList();

            var result = from book in books
                         join statuses in statusesOfUser on book.Isbn equals statuses.BookId
                         select new { Isbn = book.Isbn, Read = statuses.PagesRead, Date = statuses.DateStarted, Author = book.Author, Title = book.Title, Pages = book.Pages, Genre = book.Genre, Status = statuses.Status };


            List<ReadingStatusViewModel> data = new();
            foreach (var item in result)
            {
                Book book = new() { Isbn = item.Isbn, Genre = item.Genre, Author = item.Author, Pages = item.Pages, Title = item.Title };
                ReadingStatus statuses = new() { BookId = item.Isbn, Status = item.Status,PagesRead=item.Read,DateStarted=item.Date};

                data.Add(new() { Book = book, Status = statuses});
            } 

            return Json(new { data = data });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserBooks()
        {
            var currentUser = await GetCurrentUserAsync();

            var userId = currentUser?.Id;
            var booksIdOfCurrentUser = _db.ReadingStatuses.Where(x => x.UserId == userId);

            var pagesReadOfCurrentUserList = _db.ReadingStatuses.Where(x => x.UserId == userId);
            var allReadPages = pagesReadOfCurrentUserList.Sum(x => x.PagesRead);

            var booksOfCurrentUser = _db.Books.Where(x => booksIdOfCurrentUser.Any(b => x.Isbn == b.BookId)).ToList();
            var booksReadOfCurrentUser = _db.Books.Where(x => booksIdOfCurrentUser.Any(b => x.Isbn == b.BookId && b.Status == "Read")).ToList();

            var booksInProgressOfCurrentUser = _db.Books.Where(x => booksIdOfCurrentUser.Any(b => x.Isbn == b.BookId && b.Status == "In progress")).ToList();

            var favouriteAuthor = booksOfCurrentUser.GroupBy(s => s.Author)
                         .OrderByDescending(s => s.Count()).FirstOrDefault();
            string favouriteAuthorString = string.Empty;
            if (favouriteAuthor != null)
            {
                favouriteAuthorString  = favouriteAuthor.Key;
            }
            var firstBookId = _db.ReadingStatuses.Where(x => x.UserId == userId).OrderBy(x => x.DateStarted).Select(x => x.BookId).FirstOrDefault();

            var firstBook = _db.Books.Where(x => x.Isbn == firstBookId).Select(x => x.Title).FirstOrDefault();



            Statistic statistic = new Statistic();
            statistic.AmountOfUserBooks = booksOfCurrentUser.Count;
            statistic.BooksRead = booksReadOfCurrentUser.Count;
            statistic.BooksInProgress = booksInProgressOfCurrentUser.Count;
            statistic.PagesRead = allReadPages;
            statistic.FavouriteAuthor = favouriteAuthorString;
            statistic.FirstBook = firstBook;
            return View(statistic);

        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [BindProperty]
        public Book Book { get; set; }

        [Authorize]
        public async Task<IActionResult> Upsert(int? id)
        {
            CreatingViewModel model = new();
            if (id is null) return View(model);

            var book = await _db.Books.FindAsync(id);
            var status = await _db.ReadingStatuses.FirstOrDefaultAsync(u => u.BookId == id);
            model.Book = book;
            model.Status = status;
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create() => View(new Book());
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBook(Book book)
        {
            if (ModelState.IsValid)
            {
                Book bookfromDB = await _db.Books.FindAsync(book.Isbn);
                await _db.AddAsync(book);
               

                await _db.SaveChangesAsync();
            }
            return Redirect("/Home/GetBooks");
        }

        [Authorize]
        public async Task<IActionResult> AddStatus(int id)
        {
            var currentUser = await GetCurrentUserAsync();

            var userId = currentUser?.Id;
            ReadingStatus status = new ReadingStatus()
            { BookId = id,UserId= userId, Status = "In progress",DateStarted=DateTime.Now};
            var k = status;
            var statusFromDb = await _db.ReadingStatuses.FirstOrDefaultAsync(u => u.BookId == id && u.UserId==userId);
            if (statusFromDb==null)
            {
                await _db.ReadingStatuses.AddAsync(status);
                await _db.SaveChangesAsync();
            }

            return Redirect("/Home/GetBooks");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CreatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                Book bookfromDB = await _db.Books.FindAsync(model.Book.Isbn);
                ReadingStatus status = await _db.ReadingStatuses.FirstOrDefaultAsync(u => u.BookId == model.Book.Isbn);
                if (bookfromDB is null)
                    await _db.AddAsync(model.Book);
                else 
                {
                    status.PagesRead = model.Status.PagesRead;
                    status.Status = model.Status.Status;

                }

                await _db.SaveChangesAsync();
            }
            return Redirect("/Home/UserBooks");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await GetCurrentUserAsync();

            var userId = currentUser?.Id;
            var readingStatus = await _db.ReadingStatuses.Where(u => u.BookId == id && u.UserId== userId).FirstOrDefaultAsync();
            _db.ReadingStatuses.Remove(readingStatus);
            await _db.SaveChangesAsync();

            return Json(new { success = true, msg = "Deleted successfully" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

   
}
