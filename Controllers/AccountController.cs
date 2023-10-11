using ApiEcomm.Data;
using ApiEcomm.Extensions;
using ApiEcomm.Models;
using ApiEcomm.Services;
using ApiEcomm.ViewModels;
using ApiEcomm.ViewModels.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace ApiEcomm.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model,
                                              [FromServices] EmailService emailService,
                                              [FromServices] ApiDbContext context)
        {
            if (!ModelState.IsValid)
                BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };
            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);
            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                emailService.Send(user.Name,
                                  user.Email,
                                  "Bem vindo a Loja! Obrigado por se cadastrar.",
                                  $"Sua senha é {password}");

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("Este email ja está cadastrado."));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("Falha interna do servidor."));
            }
        }
        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model,
                                               [FromServices] ApiDbContext context,
                                               [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            var user = await context.Users.AsNoTracking()
                                          .Include(x => x.Roles)
                                          .FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos."));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos."));
            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor."));
            }

        }
    }
}
