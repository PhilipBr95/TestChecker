//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddRazorPages();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapRazorPages();

//app.Run();



using Microsoft.Owin.Hosting;
using System.Net.Http;
using WebAppOwin;

string baseAddress = "http://localhost:9000/";

// Start OWIN host 
using (WebApp.Start<Startup>(url: baseAddress))
{
    // Create HttpClient and make a request to api/values 
    HttpClient client = new HttpClient();

    var response = client.GetAsync(baseAddress + "api/values").Result;

    Console.WriteLine(response);
    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
    Console.ReadLine();
}