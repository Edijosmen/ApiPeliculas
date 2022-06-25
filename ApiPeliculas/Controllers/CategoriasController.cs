


using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliuclaCategoria")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _ctrRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepository ctrRepo, IMapper mapper)
        {
            _ctrRepo = ctrRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// Obtener la lista de todad las categorias
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCategoria()
        {
            var listCategoria = _ctrRepo.GetCategorias();
            var listCategoriaDto = _mapper.Map<List<CategoriaDto>>(listCategoria);
            return Ok(listCategoriaDto);
        }
        [AllowAnonymous]
        [HttpGet("{Id:int}", Name = "GetCategoria")]
        public IActionResult GetCategoria(int Id)
        {
            var itemCategoria = _ctrRepo.GetCategoria(Id);
            if (itemCategoria == null)
            {
                return NotFound();
            }
            var categoria = _mapper.Map<CategoriaDto>(itemCategoria);
            return Ok(categoria);
        }
        
        [HttpPost]
        public IActionResult SetCategoria([FromBody] CategoriaDto categoriaDto)
        {
            if (categoriaDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_ctrRepo.ExisteCategoria(categoriaDto.Nombre))
            {
                ModelState.AddModelError("", "la categoria ya existe");
                return StatusCode(404, ModelState);
            }
            var categoria = _mapper.Map<Categoria>(categoriaDto);
            if (!_ctrRepo.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"algo salio mal guardando el registro{categoriaDto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return Ok();
        }
        [HttpPatch("{Id:int}", Name = "GetCategoria")]
        public IActionResult updateCategoria(int Id, [FromBody] CategoriaDto categoriaDto)
        {
            if (categoriaDto == null || Id !=categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            if (!_ctrRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"algo salio mal Actualizar el registro{categoriaDto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{Id:int}", Name = "DeleteCategoria")]
        public IActionResult DeleteCategoria(int Id)
        {
            //var categoria = _mapper.Map<Categoria>(categoriaDto);
            if (!_ctrRepo.ExisteCategoria(Id))
            {

                return NotFound();
            }
            var categoria = _ctrRepo.GetCategoria(Id);
            if (!_ctrRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"algo salio mal al borrar el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
