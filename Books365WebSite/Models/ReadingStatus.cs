
using System;
using System.Collections.Generic;

#nullable disable

namespace Books365WebSite.Models
{
    public class ReadingStatus
    {
        public int Id { get; set; }
        public int? BookId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime DateStarted { get; set; }
        public int PagesRead { get; set; }
    }
}
