# GenericRepository
A high-performance, extensible repository providing consistent CRUD operations across all entities.

## Key Features
- Supports filtering, sorting, and LINQ-based querying with IQueryable.
- Asynchronous methods for non-blocking database operations.
- Automatically handles auditable fields (CreatedAt, UpdatedAt, DeletedAt, etc.).
- Built-in paging for efficient large data handling.
- Supports both soft delete (auto-excluded from queries) and force delete.
- Includes transaction support for safe multi-step operations.
- Ensures database existence before performing operations.
- Flexible design allowing custom repository implementations per entity.


```code
    await repo.GetAllAsync(); // excludes IsDeleted
    await repo.GetAllAsync(false); // includes all

```

## How to Use

### Method : 1

```code
public class UnitOfWork(AppDbContext context) : UnitOfWork<AppDbContext>(context)
{
}
```

> In Program.cs

```code
services.AddTransient<IUnitOfWork, UnitOfWork>();
```

### Method : 2

```code
public interface IProductRepository : IRepository<Product>
{
    bool Status();
}

public class ProductRepository(TestDbContext context)
    : Repository<Product, TestDbContext>(context), IProductRepository
{
    public bool Status()
        => true;
}
```

```code
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        Categories = new Repository<Category, AppDbContext>(_context);
        Products = new ProductRepository(_context);
    }

    public IRepository<Category> Categories { get; private set; }
    public IRepository<Product> Products { get; private set; }


    public void Dispose()
        => _context.Dispose();

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
```

> In Endpoint OR Controller

```code
app.MapGet("/api/products", async (IUnitOfWork repo) =>
{
    var products = await repo.Of<Product>().RecordsAsync();
    return Results.Ok(products);
});

app.MapGet("/api/products/{id:int}", async (int id, IUnitOfWork repo) =>
{
    var products = await repo.Of<Product>().FindByIdAsync(id);
    return Results.Ok(products);
});

app.MapGet("/api/products/{id:int}", async (int id, IUnitOfWork repo) =>
{
    var products = await repo.Of<Product>().FirstOrDefaultAsync(x=>x.Id == id);
    return Results.Ok(products);
});

app.MapGet("/api/products/{name}", async (string name, IUnitOfWork repo) =>
{
    var products = await repo.Of<Product>().FindByNameAsync(name);
    return Results.Ok(products);
});

app.MapGet("/api/products/{name}", async (string name, IUnitOfWork repo) =>
{
    var products = await repo.Of<Product>().FirstOrDefaultAsync(x=>x.Name == name);
    return Results.Ok(products);
});


app.MapPost("/api/product/create", async (IUnitOfWork repo, Product product) =>
{
    await repo.Of<Product>().AddAsync(product);
    await repo.SaveChangesAsync()

    return Results.Ok(new { Product = product, Status = "Success" });
});

app.MapPost("/api/product/{id:int}", async (int id, IUnitOfWork repo) =>
{
    var product = await repo.Of<Product>().Entity()
                                    .Include(x=>x.Category)
                                    .Where(x=>x.Id == id)
                                    .FirstOrDefaultAsync();

    var products = await repo.Of<Product>().Entity()
                                    .Include(x=>x.Category)
                                    .Where(x=>x.Id == id)
                                    .ToListAsync();

    return Results.Ok(new { Product = product, Status = "Success" });
});
``