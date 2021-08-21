using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("TodoItems"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "Hello World!");

app.MapGet("/todoitems", async (HttpContext http, TodoDbContext dbContext) =>
{
    var todoItems = await dbContext.TodoItems.ToListAsync();
    await http.Response.WriteAsJsonAsync(todoItems);
});

app.MapGet("/todoitems/{id}", async (int id, HttpContext http, TodoDbContext dbContext) =>
{
    var todoItem = await dbContext.TodoItems.FindAsync(id);

    if (todoItem == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    await http.Response.WriteAsJsonAsync(todoItem);
});

app.MapPost("/todoitems", async (TodoItem todoItem, HttpContext http, TodoDbContext dbContext) =>
{
    dbContext.TodoItems.Add(todoItem);
    await dbContext.SaveChangesAsync();
    http.Response.StatusCode = 204;
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem inputTodoItem, HttpContext http, TodoDbContext dbContext) =>
{
    var todoItem = await dbContext.TodoItems.FindAsync(id);

    if (todoItem == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    todoItem.IsCompleted = inputTodoItem.IsCompleted;
    await dbContext.SaveChangesAsync();
    http.Response.StatusCode = 204;
});

app.MapDelete("/todoitems/{id}", async (int id, HttpContext http, TodoDbContext dbContext) =>
{
    var todoItem = await dbContext.TodoItems.FindAsync(id);

    if (todoItem == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    dbContext.TodoItems.Remove(todoItem);
    await dbContext.SaveChangesAsync();

    http.Response.StatusCode = 204;
});

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
