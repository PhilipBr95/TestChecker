using WebApplicationChild;
using TestChecker.Runner;
using TestChecker.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTestEndpoint();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseTestEndpoint<MyTestData>(new List<ITestCheckDependency> {
    new TestCheckDependency(new WebApplicationChildChildClient.ChildChildClient("https://localhost:7264")) }, () => new MyTestChecks());

app.Run();
