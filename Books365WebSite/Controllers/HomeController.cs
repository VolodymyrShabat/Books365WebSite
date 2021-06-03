using Books365WebSite.Infrustructure;
using Books365WebSite.Interfaces;
using Books365WebSite.Models;
using Books365WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public Book Book { get; set; }

        public IActionResult Index() => View();

        [Authorize]
        public IActionResult GetBooks() => View();

        [HttpGet]
        public async Task<IActionResult> Books() => Json(new { data = await _repository.GetAllBooks() });

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserBooks()
        {
            var data = await _repository.GetUserBooks(HttpContext.User);
            return Json(new { data = data });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserBooks()
        {
            var statistic = await _repository.GetStatistic(HttpContext.User);
            return View(statistic);

        }


        [Authorize]
        public async Task<IActionResult> Upsert(int? id)
        {
            CreatingViewModel model = new();
            if (id is null) return View(model);

            var book = await _repository.GetBookById(id);
            var status = await _repository.GetBookReadingStatus(id, HttpContext.User);
            model.Book = book;
            model.Status = status;
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create() =>  View(new Book());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBook(Book book)
        {
            if (ModelState.IsValid)
            {
                Book bookfromDB = await _repository.GetBookById(book.Isbn);
                await _repository.AddBook(book);
            }

            return Redirect("/Home/GetBooks");
        }

        [Authorize]
        public async Task<IActionResult> AddStatus(int id)
        {
            var currentUser = _repository.GetCurrentUserAsync(HttpContext.User);
            var userId = currentUser?.Result.Id;
            ReadingStatus status = new ReadingStatus()
            { BookId = id, UserId = userId.ToString(), Status = "In progress", DateStarted = DateTime.Now };

            var statusFromDb = await _repository.GetBookReadingStatus(id, HttpContext.User);
            if (statusFromDb == null)
                await _repository.AddReadingStatus(status);


            return Redirect("/Home/GetBooks");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CreatingViewModel model)
        {
            if (ModelState.IsValid)
                await _repository.UpdateReadingStatus(model, HttpContext.User);

            return Redirect("/Home/UserBooks");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteReadingStatus(id, HttpContext.User);
            return Json(new { success = true, msg = "Deleted successfully" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}