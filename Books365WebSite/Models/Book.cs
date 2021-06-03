using System;
using System.Collections.Generic;

#nullable disable

namespace Books365WebSite.Models
{
    public class Book
    {

        public int Isbn { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
        public int? Pages { get; set; }
        public string Author { get; set; }
    }
}
