using BooksPlant.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksPlant.Filters;
using BooksPlant.Utilities;
using Microsoft.Extensions.FileProviders;
using Nest;
using System.IO;
namespace BooksPlant
 {
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseNpgsql(
                  Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddRazorPages(options =>
            {
                options.Conventions
                   .AddPageApplicationModelConvention("/StreamedSingleFileUploadPhysical",
                       model =>
                       {
                           model.Filters.Add(
                               new GenerateAntiforgeryTokenCookieAttribute());
                           model.Filters.Add(
                               new DisableFormValueModelBindingAttribute());
                       });

            });
            // To list physical files from a path provided by configuration:
            var physicalProvider = new PhysicalFileProvider2(Configuration.GetValue<string>("StoredFilesPath"));

            // To list physical files in the temporary files folder, use:
            //var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());

            services.AddSingleton<IFileProvider>(physicalProvider);

            
            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 256 MB
                options.MultipartBodyLengthLimit = 268435456;
            });

            // Elasticsearch configs
            var node = new Uri("http://localhost:9200/");
            var settings = new ConnectionSettings(node).DefaultIndex("books");
            settings.ThrowExceptions(alwaysThrow: true); // I like exceptions
            settings.PrettyJson(); // Good for DEBUG

            services.AddSingleton<IElasticClient>(new ElasticClient(settings)); 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            app.UseStaticFiles(); // For the wwwroot folder.

            // using Microsoft.Extensions.FileProviders;
            // using System.IO;
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "StaticFiles")),
                RequestPath = "/StaticFiles",
                EnableDirectoryBrowsing = true
            });
        }
    }
}
