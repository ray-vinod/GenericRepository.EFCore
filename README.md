# GenericRepository

- This is a generic repository of basic **CRUD** operation
- In This read entities with expression filters, sorting and include other dependent entities
- It also return a queryable entity on which you can apply other linq operation
- In this version added GetByName() method and GetItemsAsync() change into GetAllAsync() with no predicate for detail go through examples

## How to Use

### Method : 1

```code
public interface IUnitOfWork : IDisposable
{
    // map repository name as DbSet in DbContext for convineant
    IRepository<Category> Categories { get; }
    IRepository<Product> Products { get; }

    Task<int> SaveChangesAsync();
}


public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _contex;

    public UnitOfWork(AppDbContext contex)
    {
        _contex = contex;

        Categories = new Repository<Category, AppDbContext>(_contex);
        Products = new Repository<Product, AppDbContext>(_contex);
    }


    public IRepository<Category> Categories { get; private set; }
    public IRepository<Product> Products { get; private set; }



    public void Dispose()
        => _contex.Dispose();

    public async Task<int> SaveChangesAsync()
        => await _contex.SaveChangesAsync();
}
```

> In Program.cs

```code
services.AddTransient&lt;IUnitOfWork, UnitOfWork&gt;();
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
app.MapGet("/api/products", async (IUnitOfWork unitOfWork) =>
{
    var products = await unitOfWork.Products.GetAllAsync();
    return Resutls.Ok(products);
});

app.MapGet("/api/products/{id:int}", async (int id, IUnitOfWork unitOfWork) =>
{
    var products = await unitOfWork.Products.GetByIdAsync(id);
    return Resutls.Ok(products);
});

app.MapGet("/api/products/{id:int}", async (int id, IUnitOfWork unitOfWork) =>
{
    var products = await unitOfWork.Products.FirstOrDefaultAsync(x=>x.Id == id);
    return Resutls.Ok(products);
});

app.MapGet("/api/products/{name}", async (string name, IUnitOfWork unitOfWork) =>
{
    var products = await unitOfWork.Products.GetByNameAsync(name);
    return Resutls.Ok(products);
});

app.MapGet("/api/products/{name}", async (string name, IUnitOfWork unitOfWork) =>
{
    var products = await unitOfWork.Products.FirstOrDefaultAsync(x=>x.Name == name);
    return Resutls.Ok(products);
});


app.MapPost("/api/product/create", async (IUnitOfWork unitOfWork, Product product) =>
{
    await unitOfWork.Products.AddAsync(product);
    await unitOfWork.SaveChangesAsync()

    return Results.Ok(new { Product = product, Status = "Success" });
});

app.MapPost("/api/product/{id:int}", async (int id, IUnitOfWork unitOfWork) =>
{
    var product = await unitOfWork.Products.GetEntity()
                                    .Include(x=>x.Category)
                                    .Where(x=>x.Id == id)
                                    .FirstOrDefaultAsync();

    var products = await unitOfWork.Products.GetEntity()
                                    .Include(x=>x.Category)
                                    .Where(x=>x.Id == id)
                                    .ToListAsync();

    return Results.Ok(new { Product = product, Status = "Success" });
});
```
