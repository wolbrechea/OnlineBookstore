using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{        public class BookSearchModel
        {
            public int? Page { get; set; }
            [Display(Name = "Book")]
            public string Title { get; set; }
            public List<Book> SearchResults { get; set; }
            public string SearchButton { get; set; }
        }
}