using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books365WebSite.Models
{
    public class Statistic
    {
        public Statistic(string userId)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();

            var options = optionsBuilder
                    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Books365;Trusted_Connection=True;MultipleActiveResultSets=true")
                    .Options;
            using (Context _db = new Context(options))
            {
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
                    favouriteAuthorString = favouriteAuthor.Key;
                }
                var firstBookId = _db.ReadingStatuses.Where(x => x.UserId == userId).OrderBy(x => x.DateStarted).Select(x => x.BookId).FirstOrDefault();

                var firstBook = _db.Books.Where(x => x.Isbn == firstBookId).Select(x => x.Title).FirstOrDefault();

                AmountOfUserBooks = booksOfCurrentUser.Count;
                BooksRead = booksReadOfCurrentUser.Count;
                BooksInProgress = booksInProgressOfCurrentUser.Count;
                PagesRead = allReadPages;
                FavouriteAuthor = favouriteAuthorString;
                FirstBook = firstBook;
            }
        }
        public int BooksRead { get;  }
        public int AmountOfUserBooks { get;  }
        public int BooksInProgress { get;  }
        public int PagesRead { get;  }
        public string FavouriteAuthor { get; }
        public string FirstBook { get;  }


    }
}
