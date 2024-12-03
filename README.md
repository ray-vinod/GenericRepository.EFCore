# GenericRepository

- This is a generic repository of basic **CRUD** operation
- In this read entities with expression filters, sorting and include other dependent entities
- It also return a queryable entity on which you can apply other linq extentions

## How to Use

### Method : 1

```code
public class UnitOfWork(AppDbContext context) : UnitOfWork<AppDbContext>(context)
{
}
```

> In Program.cs

```code
// &lt; for '<' and &gt; for '>'
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
    private readonly AppDbContext _contex;

    public UnitOfWork(AppDbContext contex)
    {
        _contex = contex;

        Categories = new Repository<Category, AppDbContext>(_contex);
        Products = new ProductRepository(_contex);
    }

    public IRepository<Category> Categories { get; private set; }
    public IRepository<Product> Products { get; private set; }


    public void Dispose()
        => _contex.Dispose();

    public async Task<int> SaveChangesAsync()
        => await _contex.SaveChangesAsync();
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
```
