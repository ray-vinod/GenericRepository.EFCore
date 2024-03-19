# GenericRepository

- This is a generic repository of basic **CRUD** operation
- In This read entities with expression filters, sorting and include other dependent entities
- It also return a queryable entity on which you can apply other linq operation

## How to Use

### Method : 1

```code
public interface IUnitOfWork : IDisposable
{
    IRepository&lt;Category&gt; Categories { get; }
    IRepository&lt;Product&gt; Products { get; }

    Task<int> SaveChangesAsync();
}


public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _contex;

    public UnitOfWork(AppDbContext contex)
    {
        _contex = contex;

        Categories = new Repository&lt;Category, AppDbContext7gt;(_contex);
        Products = new Repository&lt;Product, AppDbContext&gt;(_contex);
    }


    public IRepository&lt;Category&gt; Categories { get; private set; }
    public IRepository&lt;Product&gt; Products { get; private set; }



    public void Dispose()
        => _contex.Dispose();

    public async Task&lt;int&gt; SaveChangesAsync()
        => await _contex.SaveChangesAsync();
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
    private readonly AppDbContext _contex;

    public UnitOfWork(AppDbContext contex)
    {
        _contex = contex;

        Categories = new Repository&lt;Category, AppDbContext7gt;(_contex);
        Products = new ProductRepository(_contex);
    }


    public IRepository&lt;Category&gt; Categories { get; private set; }
    public IRepository&lt;Product&gt; Products { get; private set; }



    public void Dispose()
        => _contex.Dispose();

    public async Task&lt;int&gt; SaveChangesAsync()
        => await _contex.SaveChangesAsync();
}
```

> In Endpoint OR Controller

```code
app.MapGet("/api/products", async (IProductRepository productRepository) =>
{
    var products = await productRepository.GetItemsAsync()
    return products;
})
app.MapPost("/api/product/create", async (IProductRepository productRepository, Product product) =>
{
    await productRepository.CreateAsync(product);
    await productRepository.SaveChanges()
    return new { Product = product, Status = "Success" };
});
```
