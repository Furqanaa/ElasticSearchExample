using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksPlant.Models
{
    public class Book
    {
        public int Id { get; set; }
     
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string CoverImagePath { get; set; }
        public string StoragePath { get; set; }
        public string FileName { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
