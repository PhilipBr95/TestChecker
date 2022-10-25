using WebApplication1;
using TestChecker.Runner;
using TestChecker.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTestEndpoint();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseTestEndpoint<MyTestData>(new List<ITestCheckDependency> { 
    new TestCheckDependency(new WebApplicationChildClient.ChildClient("https://localhost:7254")) }, 
    () => new MyTestChecks());

app.Run();
