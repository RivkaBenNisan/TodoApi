using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using System.Web.Cors;

using TodoApi;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

//שליפת כל המשימות
app.MapGet("/", async (ToDoDbContext context) =>
{
   var data = await context.Items.ToListAsync();
   return Results.Ok(data);
});

// //ID שליפת משימה לפי 
// app.MapGet("/todos/{id}", (ToDoDbContext dbContext,int id) => dbContext.Items.Find(id));

//הוספת משימה
app.MapPost("/{name}", async (string name, ToDoDbContext dbContext)=>
{
   Item item=new Item{Name=name,IsComplete=false};
    await dbContext.AddAsync(item);
    await dbContext.SaveChangesAsync();
});
//עדכון משימה
app.MapPut("/{id}/{IsComplete}", async (bool IsComplete,ToDoDbContext dbContext,int id) =>
{
    var item = await dbContext.Items.FindAsync(id);

    if (item is null) return Results.NotFound();

    item.IsComplete = IsComplete;

    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

//מחיקת משימה
app.MapDelete("/{id}", async (int Id, ToDoDbContext dbContext) =>
{
    if (await dbContext.Items.FindAsync(Id) is Item item)
    {
        dbContext.Items.Remove(item);
        await   dbContext.SaveChangesAsync();
    }
});


app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();

