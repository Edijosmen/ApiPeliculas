using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Profiles
{
    public class PeliculasMapper:Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaCreateDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaUpdateDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioAuthDto>().ReverseMap();
            CreateMap<Usuario, UsuarioAuthLoginDto>().ReverseMap();


        }
    }
}
