using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using api_rest.Data;
using api_rest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api_rest.Data
{
    [Route("api/v1/[controller]")] 
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly ApplicationDbContext database;

        public UsuariosController(ApplicationDbContext database){
            this.database = database;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            try{
                var produtos = database.Produtos.ToList();
                return Ok(produtos);
            }catch(Exception e){
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
            
        }

        [HttpPost]
        public IActionResult Post([FromBody] Usuario usuario){
            database.Usuarios.Add(usuario);
            database.SaveChanges();
            Response.StatusCode = 201;
            return new ObjectResult("");
        }

         [HttpPost]
        public IActionResult Login([FromBody] Usuario credenciais){
            try{
                Usuario usuario = database.Usuarios.First(user => user.Email.Equals(credenciais.Email));
                if(usuario != null){
                    if(usuario.Senha.Equals(credenciais.Senha)){

                        string chaveDeSeguranca = "school_of_net";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica,SecurityAlgorithms.HmacSha256Signature);
                        
                        var claims = new List<Claim>();
                        claims.Add(new Claim("id",usuario.Id.ToString()));
                        claims.Add(new Claim("email",usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role,"Admin"));

                        var JWT = new JwtSecurityToken(
                            issuer: "school_of_net_system",
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisDeAcesso,
                            claims: claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                    } else {
                        Response.StatusCode = 401;
                        return new ObjectResult("");
                    }
                } else {
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                }
            } catch(Exception e){
                Response.StatusCode = 401;
                return new ObjectResult("");
            }
        }
    }
}