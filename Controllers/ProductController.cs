using ApiEcomm.Data;
using ApiEcomm.Extensions;
using ApiEcomm.Models;
using ApiEcomm.ViewModels;
using ApiEcomm.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiEcomm.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet("v1/products")]
        public async Task<IActionResult> GetAsync([FromServices] ApiDbContext context,
                                                  [FromQuery] int page = 0,
                                                  [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Products.AsNoTracking().CountAsync();
                var products = await context.Products.AsNoTracking()
                                                     .Include(x => x.Category)
                                                     .Select(x => new ListProductsViewModel
                                                     {
                                                         Id = x.Id,
                                                         Title = x.Title,
                                                         Description = x.Description,
                                                         Price = x.Price,
                                                         LastUpdateDate = x.LastUpdateDate,
                                                         Category = x.Category
                                                     })
                                                     .Skip(page * pageSize)
                                                     .Take(pageSize)
                                                     .OrderByDescending(x => x.Id)
                                                     .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    products
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor."));
            }
        }
        [HttpGet("v1/products/{id:int}")]
        public async Task<IActionResult> DetailAsync([FromServices] ApiDbContext context,
                                                     [FromRoute] int id)
        {
            try
            {
                var products = await context.Products.AsNoTracking()
                                                     .Include(x => x.Category)
                                                     .FirstOrDefaultAsync(x => x.Id == id);
                if (products == null)
                    return NotFound(new ResultViewModel<Product>("Conteúdo não encontrado."));

                return Ok(new ResultViewModel<Product>(products));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor."));
            }
        }
        [HttpGet("products/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync([FromServices] ApiDbContext context,
                                                            [FromRoute] string category,
                                                            [FromQuery] int page = 0,
                                                            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Products.AsNoTracking().CountAsync();
                var products = await context.Products.AsNoTracking()
                                                     .Include(x => x.Category)
                                                     .Where(x => x.Category.Title == category)
                                                     .Select(x => new ListProductsViewModel
                                                     {
                                                         Id = x.Id,
                                                         Title = x.Title,
                                                         Description = x.Description,
                                                         Price = x.Price,
                                                         LastUpdateDate = x.LastUpdateDate,
                                                         Category = x.Category
                                                     })
                                                     .Skip(page * pageSize)
                                                     .Take(pageSize)
                                                     .OrderByDescending(x => x.Id)
                                                     .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    products
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor."));
            }
        }
        [HttpPost("v1/products")]
        public async Task<IActionResult> PostProduct([FromBody] CreateProductViewModel model,
                                                     [FromServices] ApiDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Product>(ModelState.GetErrors()));
            try
            {
                var product = new Product
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    LastUpdateDate = model.LastUpdateDate,
                    CategoryId = model.CategoryId,
                    Category = await context.Categories.AsNoTracking()
                                                       .FirstOrDefaultAsync(x => x.Id == model.CategoryId)
                };
                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();
                return Created($"v1/products/{product.Id}", new ResultViewModel<Product>(product));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Product>("Não foi possivel Inserir a categoria."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor."));
            }
        }
        [HttpPut("v1/products/{id:int}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id,
                                                    [FromBody] EditProductViewModel model,
                                                    [FromServices] ApiDbContext context)
        {
            try
            {
                var products = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (products == null)
                    return NotFound(new ResultViewModel<Product>("Conteudo não encontrado."));
                products.Title = model.Title;
                products.Description = model.Description;
                products.Price = model.Price;
                products.CategoryId = model.CategoryId;
                products.Category = await context.Categories.AsNoTracking()
                                                            .FirstOrDefaultAsync(x => x.Id == model.CategoryId);

                context.Products.Update(products);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Product>(products));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Product>("Não foi possivel Inserir a categoria."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor."));
            }
        }
        [HttpDelete("v1/products/{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id,
                                                       [FromServices] ApiDbContext context)
        {
            try
            {
                var products = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (products == null)
                    return NotFound(new ResultViewModel<Product>("Conteudo não encontrado."));
                context.Products.Remove(products);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Product>(products));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Product>("Não foi possivel Inserir a categoria."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor."));
            }
        }
    }
}