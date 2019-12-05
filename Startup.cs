using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConvertItOnline
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                 {
                     await context.Response.WriteAsync(@"Loading ...");
                     context.Response.Redirect("/index.html", true);
                 });
                endpoints.MapGet("/text", async context =>
                {
                    await context.Response.WriteAsync(File.ReadAllText("./text.txt"));
                });
                //endpoints.MapRazorPages();*/
                endpoints.MapPost("/Process", async context =>
                {
                    await context.Response.WriteAsync("");
                    ProcessController.AcceptRequest(context);
                });
                /*async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                    var req = context.Request;
                    string formItems = "";
                    foreach (var key in req.Form.Keys)
                    {
                        formItems += key + req.Form[key];
                    }
                    string formfiles = "";
                    foreach (var file in req.Form.Files)
                    {
                        formfiles += file.Name + file.Length;
                    }
                    File.WriteAllText("headers.bin", formItems);
                    File.WriteAllText("files.bin", formfiles);
                    //File.WriteAllText("headers.bin", req.Form.Files.GetFile(""));
                    File.WriteAllBytes("input.bin", ReadStream(req.Body));
                    readImage(req.Form.Files[0]).Save("input.png");
                    /*var reader = new StreamReader(context.Request.Body);
                    System.Console.WriteLine(reader.ReadToEnd());
                });*/
            });
        }
        private byte[] ReadStream(Stream s)
        {
            var reader = new StreamReader(s);
            string body = reader.ReadToEndAsync().Result;
            /*while (!reader.EndOfStream)
            {
                output+=reader.readd
            }*/
            var output = new byte[body.Length];
            for (int i = 0; i < body.Length; i++)
            { output[i] = (byte)body[i]; }
            return output;
        }
        private Image readImage(IFormFile file) => Bitmap.FromStream(file.OpenReadStream());
    }
}
