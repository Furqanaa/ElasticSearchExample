using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using BooksPlant.Utilities;
using BooksPlant.Models;
using System;
using BooksPlant.Data;

namespace BooksPlant.Pages
{
    public class BufferedDoubleFileUploadPhysicalModel : PageModel
    {
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".txt" , ".pdf", ".png", ".jpg"};
        private readonly string _targetFilePath;
        private readonly ApplicationDbContext _context;
        public BufferedDoubleFileUploadPhysicalModel(IConfiguration config, ApplicationDbContext context)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");

            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
            _context = context;

            // To save physical files to the temporary files folder, use:
            //_targetFilePath = Path.GetTempPath();
        }

        [BindProperty]
        public BookForm UploadBookForm { get; set; }

        public string Result { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            // Prepare the book model to be stored later in the DB.
            Book book = new Book();
            book.Title = UploadBookForm.Title;
            book.Author = UploadBookForm.Author;
            book.PublishDate = UploadBookForm.IssueDate;
            book.Description = UploadBookForm.Description;

            var formFiles = new List<IFormFile>()
            {
                UploadBookForm.PdfFile,
                UploadBookForm.ImageFile
            };

            // Upload the Pdf file and the image file to the physical storage:
            foreach (var formFile in formFiles)
            {
                var formFileContent =
                    await FileHelpers
                        .ProcessFormFile<BookForm>(
                            formFile, ModelState, _permittedExtensions,
                            _fileSizeLimit);

                if (!ModelState.IsValid)
                {
                    Result = "Please correct the form.";

                    return Page();
                }


                // For the file name of the uploaded file stored
                // server-side, use Path.GetRandomFileName to generate a safe
                // random file name.
                var trustedFileNameForFileStorage = Path.GetRandomFileName();
                var filePath = Path.Combine(
                    _targetFilePath, trustedFileNameForFileStorage + Path.GetExtension(formFile.FileName).ToLowerInvariant());

                // **WARNING!**
                // In the following example, the file is saved without
                // scanning the file's contents. In most production
                // scenarios, an anti-virus/anti-malware scanner API
                // is used on the file before making the file available
                // for download or for use by other systems. 
                // For more information, see the topic that accompanies 
                // this sample.

                using (var fileStream = System.IO.File.Create(filePath))
                {
                    await fileStream.WriteAsync(formFileContent);

                    // To work directly with the FormFiles, use the following
                    // instead:
                    //await formFile.CopyToAsync(fileStream);
                }
                // Depends 
                if (Path.GetExtension(formFile.FileName).ToLowerInvariant().Equals(".pdf"))
                {
                    book.StoragePath = filePath;
                    book.FileName = formFile.FileName;
                }
                else
                {
                    book.CoverImagePath = filePath;
                }
                
            }
            // Use DB context here to save the fucking book
            _context.Add(book);
            _context.SaveChanges();
            return RedirectToPage("./Index");
        }
    }

    public class BookForm
    {
        [Required]
        [Display(Name = "Pdf File")]
        public IFormFile PdfFile { get; set; }

        [Required]
        [Display(Name = "Cover Image")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Title")]
        [StringLength(50, MinimumLength = 0)]
        public string Title { get; set; }

        [Display(Name = "Author")]
        [StringLength(50, MinimumLength = 0)]
        public string Author { get; set; }


        [Display(Name = "Issue Date")]
        public DateTime IssueDate { get; set; }


        [Display(Name = "Description")]
        [StringLength(50, MinimumLength = 0)]
        public string Description { get; set; }

        [Display(Name = "Note")]
        [StringLength(50, MinimumLength = 0)]
        public string Note { get; set; }
    }
}
