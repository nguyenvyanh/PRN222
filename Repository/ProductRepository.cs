using Microsoft.EntityFrameworkCore;
using Project_Group3.Models;
using Project_Group3.Repository.Interfaces;

namespace Project_Group3.Repository;

public sealed class ProductRepository(CloneEbayDbContext dbContext) : IProductRepository
{
    public async Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(
        string? keyword,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var q = dbContext.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            q = q.Where(x =>
                (x.title ?? string.Empty).Contains(kw) ||
                (x.description ?? string.Empty).Contains(kw));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var productStatus = status.Trim();
            q = q.Where(x => x.status == productStatus);
        }

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(x => x.id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<bool> HideAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.id == id, cancellationToken);
        if (product is null) return false;

        product.status = $"hidden: {reason}";

        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.id == id, cancellationToken);
        if (product is null) return false;

        dbContext.Products.Remove(product);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
