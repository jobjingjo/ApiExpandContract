using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using System;
using System.Linq;
using System.Threading.Tasks;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader =
    ApiVersionReader.Combine(
       new HeaderApiVersionReader(new[] { "X-Api-Version" }),
       new QueryStringApiVersionReader(new[] { "version" }),
       new MediaTypeApiVersionReader("v"));
});
/*
builder.Services.AddVersionedApiExplorer(options =>
{
    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service  
    // note: the specified format code will format the version as "'v'major[.minor][-status]"  
    options.GroupNameFormat = "'v'VVV";

    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat  
    // can also be used to control the format of the API version in route templates  
    options.SubstituteApiVersionInUrl = true;
});*/

builder.Services.AddSwaggerGen(setup => {
    setup.SwaggerDoc("v1", new OpenApiInfo()
    {
        Description = "Todo web api implementation using Minimal Api in Asp.Net Core",
        Title = "Todo Api",
        Version = "v1",
        Contact = new OpenApiContact()
        {
            Name = "job",
            Url = new Uri("https://jobjingjo.github.io/")
        }
    });
    setup.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapGet("/", (Func<string>)(() => "Hello World!"));
app.MapGet("/test", (Func<string>)(() => "Hello World!"));
app.MapPost("/test", (Func<IResult>)(() => {
    return Results.Ok();
}));




/*
 * 
 * app.MapGet("/todoitems", async([FromServices] TodoDbContext dbContext) =>
{
    await dbContext.TodoItems.ToListAsync();
});
app.MapGet("/todoitems/{id}", async([FromServices] TodoDbContext dbContext, int id) =>
{
    return await dbContext.TodoItems.FindAsync(id) is TodoItem todo ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todoitems", async([FromServices] TodoDbContext dbContext, TodoItem todoItem) =>
{
    dbContext.TodoItems.Add(todoItem);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
});

app.MapPut("/todoitems/{id}", async([FromServices] TodoDbContext dbContext, int id, TodoItem inputTodoItem) =>
{
    var todoItem = await dbContext.TodoItems.FindAsync(id);
    if (todoItem == null)
    {
        return Results.NotFound();
    }

    todoItem.IsCompleted = inputTodoItem.IsCompleted;
    await dbContext.SaveChangesAsync();
    return Results.Status(204);
});

app.MapDelete("/todoitems/{id}", async([FromServices] TodoDbContext dbContext, int id) =>
{
    var todoItem = await dbContext.TodoItems.FindAsync(int.Parse(id.ToString()));
    if (todoItem == null)
    {
        return Results.NotFound();
    }

    dbContext.TodoItems.Remove(todoItem);
    await dbContext.SaveChangesAsync();

    return Results.Status(204);
});
*/

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo Api v1");
    c.RoutePrefix = string.Empty;
    /*
    // build a swagger endpoint for each discovered API version  
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
    */
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions options) : base(options)
    {
    }

    protected TodoDbContext()
    {
    }
    public DbSet<TodoItem> TodoItems { get; set; }
}
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

public static class Results
{
    public static IResult NotFound() => new StatusCodeResult(404);
    public static IResult Ok() => new StatusCodeResult(200);
    public static IResult Status(int statusCode)
        => new StatusCodeResult(statusCode);
    public static IResult Ok(object value) => new JsonResult(value);
    public static IResult Created(string location, object value)
        => new CreatedResult(location, value);

    private class CreatedResult : IResult
    {
        private readonly object _value;
        private readonly string _location;

        public CreatedResult(string location, object value)
        {
            _location = location;
            _value = value;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status201Created;
            httpContext.Response.Headers.Location = _location;

            return httpContext.Response.WriteAsJsonAsync(_value);
        }
    }
}