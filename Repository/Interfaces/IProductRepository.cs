using Project_Group3.Models;

namespace Project_Group3.Repository.Interfaces;

public interface IProductRepository
{
    Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(
        string? keyword,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> HideAsync(int id, string reason, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
