using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPelicula")]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepository _pelRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PeliculasController(IPeliculaRepository pelRepo, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// lista de peliculas
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetPelicula()
        {
            var listPelicula = _pelRepo.GetPeliculas();
            var listPeliculaDto = _mapper.Map<List<PeliculaDto>>(listPelicula);
            return Ok(listPeliculaDto);
        }

        [HttpGet("{Id:int}", Name = "GetPelicula")]
        public IActionResult GetPelicula(int Id)
        {
            var itemPelicula = _pelRepo.GetPelicula(Id);
            if (itemPelicula == null)
            {
                return NotFound();
            }
            var pelicula = _mapper.Map<PeliculaDto>(itemPelicula);
            return Ok(pelicula);
        }
        [AllowAnonymous]
        [HttpGet("GetPeliculaPorCategoria/{CatgId:int}")]
        public IActionResult GetPeliculaPorCategoria(int CatgId)
        {

            var pelicula = _pelRepo.GetPeliculasEnCategoria(CatgId);
            if(pelicula == null)
            {
                return NotFound();
            }
            var listPelicula = _mapper.Map<List<PeliculaDto>>(pelicula);
            return StatusCode(200, ModelState);
        }
        [AllowAnonymous]
        [HttpGet("Buscar")]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var result = _pelRepo.BuscarPelicula(nombre);
                if (result.Any())
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "erro recupara datos");
            }
        }
        [HttpPost]
        public IActionResult CreatePelicula([FromForm] PeliculaCreateDto peliculaDto)
        {
            if (peliculaDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_pelRepo.ExistePelicula(peliculaDto.Nombre))
            {
                ModelState.AddModelError("", "la pelicula ya existe");
                return StatusCode(404, ModelState);
            }
            //subida de archivo
            var archivoFoto = peliculaDto.Foto;
            string rutaPrincipal = _webHostEnvironment.WebRootPath;
            var archivo = HttpContext.Request.Form.Files;
            if (archivoFoto.Length > 0)
            {

                var nombreFoto = Guid.NewGuid().ToString();
                var subida = Path.Combine(rutaPrincipal, @"fotos");
                var extension = Path.GetExtension(archivo[0].FileName);

                using (var fileStram = new FileStream(Path.Combine(subida, nombreFoto + extension), FileMode.Create))
                {
                    archivo[0].CopyTo(fileStram);
                }
                peliculaDto.RutaImagen = @"\fotos\" + nombreFoto + extension;
            }
            
            var pelicula = _mapper.Map<Pelicula>(peliculaDto);
            if (!_pelRepo.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"algo salio mal guardando el registro{peliculaDto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return Ok();
        }

      


        [HttpPatch("{Id:int}", Name = "updatePelicula")]
        public IActionResult updatePelicula(int Id, [FromBody] PeliculaDto peliculaDto)
        {
            if (peliculaDto == null || Id !=peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);
            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"algo salio mal Actualizar el registro{peliculaDto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{Id:int}", Name = "DeletePelicula")]
        public IActionResult DeletePelicula(int Id)
        {
            //var pelicula = _mapper.Map<Categoria>(peliculaDto);
            if (!_pelRepo.ExistePelicula(Id))
            {

                return NotFound();
            }
            var pelicula = _pelRepo.GetPelicula(Id);
            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"algo salio mal al borrar el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
