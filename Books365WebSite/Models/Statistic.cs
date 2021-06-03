using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books365WebSite.Models
{
    public class Statistic
    {
        public int BooksRead { get; set; }
        public int AmountOfUserBooks { get; set; }
        public int BooksInProgress { get; set; }
        public int PagesRead { get; set; }
        public string FavouriteAuthor { get; set; }
        public string FirstBook { get; set; }


    }
}
