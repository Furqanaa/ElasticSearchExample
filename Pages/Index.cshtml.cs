using BooksPlant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksPlant.Models;
using Nest;

namespace BooksPlant.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        private readonly IElasticClient client;

        public ISearchResponse<Book> Search { get; set; }
        public bool HasSearch => Search != null;

        [BindProperty(SupportsGet = true)]
        public string term { get; set; }
        public int from { get; set; }
        public int pageSize { get; set; }


        public IndexModel(IElasticClient client)
        {
            this.client = client;
        }

        public void OnGet()
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                // Filter the books based on the term:
                Search =
                    client.Search<Book>(s => s  
                         .Index("books")
                         .Query(q => q
                                .MultiMatch(m => m
                                    .Fields(f => f.Field(p => p.Title).Field(p => p.Author).Field(p => p.Description))
                                    .Query(term)
                                    .Fuzziness(Fuzziness.EditDistance(1))
                                )
                            )
                        .Take(10));

            }
            else
            {
                // Simply Get all the books:
                this.GetBooks();
            }
        }
        public void GetBooks()
        {
            this.Search = client.Search<Book>(s => s
                          .Index("books")
                          //.From(from)
                          //.Size(pageSize)
                          .MatchAll());
        }
        protected void bttnpdf_Click(object sender, EventArgs e)
        {
            Console.WriteLine("heeeeey");
        }


    }
}
