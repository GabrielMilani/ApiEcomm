using ApiEcomm.Data;
using ApiEcomm.Extensions;
using ApiEcomm.Models;
using ApiEcomm.ViewModels;
using ApiEcomm.ViewModels.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ApiEcomm.Controllers
{
    [ApiController]
    public class RoleController : ControllerBase
    {
        [HttpGet("v1/roles")]
        public async Task<IActionResult> GetAsync([FromServices] ApiDbContext context,
                                                  [FromServices] IMemoryCache cache)
        {
            try
            {
                var roles = cache.GetOrCreate("RoleCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetRoles(context);
                });
                return Ok(new ResultViewModel<List<Role>>(roles));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Role>>("Falha interna no servidor."));
            }
        }
        private List<Role> GetRoles(ApiDbContext context)
        {
            return context.Roles.ToList();
        }
        [HttpGet("v1/roles/{id:int}")]
        public async Task<IActionResult> GetAsync([FromRoute] int id,
                                                  [FromServices] ApiDbContext context)
        {
            try
            {
                var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (role == null)
                    return NotFound(new ResultViewModel<Role>("Conteudo não encontrado."));
                return Ok(new ResultViewModel<Role>(role));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Role>>("Falha interna no servidor."));
            }
        }
        [HttpPost("v1/roles")]
        public async Task<IActionResult> PostAsync([FromBody] EditorRoleViewModel model,
                                                   [FromServices] ApiDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Role>(ModelState.GetErrors()));
            try
            {
                var role = new Role
                {
                    Id = 0,
                    Title = model.Title,
                    Slug = model.Slug.ToLower()
                };
                await context.Roles.AddAsync(role);
                await context.SaveChangesAsync();

                return Created($"v1/roles/{role.Id}", new ResultViewModel<Role>(role));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Role>("Não foi possivel Inserir a role."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Role>("Falha interna no servidor."));
            }
        }
        [HttpPut("v1/roles/{id:int}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id,
                                                  [FromBody] EditorRoleViewModel model,
                                                  [FromServices] ApiDbContext context)
        {
            try
            {
                var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (role == null)
                    return NotFound(new ResultViewModel<Role>("Conteudo não encontrado."));
                role.Title = model.Title;
                role.Slug = model.Slug;

                context.Roles.Update(role);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Role>(role));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Role>("Não foi possivel alterar a categoria."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Role>>("Falha interna no servidor."));
            }
        }
        [HttpDelete("v1/roles/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id,
                                                     [FromServices] ApiDbContext context)
        {
            try
            {
                var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (role == null)
                    return NotFound(new ResultViewModel<Role>("Conteudo não encontrado."));
                context.Roles.Remove(role);
                await context.SaveChangesAsync();

                return Ok(role);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Role>("Não foi possivel deletar a categoria."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Role>>("Falha interna no servidor."));
            }
        }
    }
}
