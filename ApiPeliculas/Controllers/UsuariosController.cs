using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculaUsuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public UsuariosController(IUsuarioRepository userRepo,IMapper mapper, IConfiguration config)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _config = config;
        }

        [HttpGet]
        public IActionResult  GetUsuarios()
        {
            var listUsuarios = _userRepo.GetUsuarios();
            var lisUsuarioDto = _mapper.Map<List<UsuarioDto>>(listUsuarios);
            return Ok(lisUsuarioDto);
        }
        /// <summary>
        /// listado de usuarios
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        [HttpGet("{usuarioId:int}",Name ="GetUsuario")]
        public IActionResult GetUsuario(int usuarioId)
        {
            var usuario = _userRepo.GetUsuario(usuarioId);
            if(usuario== null)
            {
                return NotFound();
            }
            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return Ok(usuarioDto);
        }
        [AllowAnonymous]
        [HttpPost("Crear",Name ="Registrar")]
        public IActionResult Registrar(UsuarioAuthDto usuarioAuth)
        {
            usuarioAuth.User = usuarioAuth.User.ToLower();
            if (_userRepo.ExisteUsuario(usuarioAuth.User))
            {
                return BadRequest("el Usuario ya existe");
            }

            var crearUsuario = new Usuario
            {
                User = usuarioAuth.User

            };
            var usuarioCreado = _userRepo.RegistroUsuario(crearUsuario, usuarioAuth.Password);
            return Ok(new {StatusCode=200, request= usuarioCreado });

        }
        [AllowAnonymous]
        [HttpPost("Login",Name ="Login")]
        public IActionResult Login(UsuarioAuthLoginDto usuarioAuthLoginDto)
        {
            var usuarioDesRepo = _userRepo.Login(usuarioAuthLoginDto.User, usuarioAuthLoginDto.Password);
          
            if(usuarioDesRepo == null)
            {
                ModelState.AddModelError("Error", $"Unauthorized");
                var response = new JsonResult(new
                {
                    StatusCode = 401,
                    SerializerSettings = ModelState,
                    Value = "Unauthorized" 
                });

                ModelState.AddModelError("Error", $"Unauthorized");
                return response;
                //return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,usuarioAuthLoginDto.Id.ToString()),
                new Claim(ClaimTypes.Name, usuarioAuthLoginDto.User.ToString())
            };
            //Generador de Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return Ok(new { StatusCode = 200,response=new { token = tokenHandler.WriteToken(token) } });
        }
    }
}
