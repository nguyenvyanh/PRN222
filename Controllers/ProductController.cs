using Microsoft.AspNetCore.Mvc;
using Project_Group3.Repository.Interfaces;

namespace Project_Group3.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductRepository productRepository) : ControllerBase
{
    public sealed record HideProductRequest(string Reason);

    [HttpGet("paged")]
    public async Task<ActionResult<object>> GetPaged(
        [FromQuery] string? keyword,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var (items, total) = await productRepository.GetPagedAsync(keyword, status, page, pageSize, cancellationToken);
        return Ok(new { items, total, page, pageSize });
    }

    [HttpPost("hide/{id:int}")]
    public async Task<IActionResult> Hide(int id, [FromBody] HideProductRequest request, CancellationToken cancellationToken)
        => await productRepository.HideAsync(id, request.Reason, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => await productRepository.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}