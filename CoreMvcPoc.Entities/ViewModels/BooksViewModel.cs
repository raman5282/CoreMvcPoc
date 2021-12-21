using System.Collections.Generic;

namespace CoreMvcPoc.Entities
{
    public class BooksViewModel
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<Book> Items { get; set; }
    }
}