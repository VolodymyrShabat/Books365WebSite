using Books365WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books365WebSite.ViewModels
{

    public class ReadingStatusViewModel
    {
        public Book Book { get; set; }
        public ReadingStatus Status { get; set; }
    }
}
