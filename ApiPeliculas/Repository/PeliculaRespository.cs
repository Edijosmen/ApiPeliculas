using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class PeliculaRespository : IPeliculaRepository
    {
        private readonly AplicationsDbContext _db;
        public PeliculaRespository(AplicationsDbContext db)
        {
            _db = db;
        }
        public bool ActualizarPelicula(Pelicula pelicula)
        {
            _db.Pelicula.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _db.Pelicula.Remove(pelicula);
            return Guardar();
        }

        public ICollection<Pelicula> BuscarPelicula(string Nombre)
        {
            IQueryable<Pelicula> query = _db.Pelicula;
            if (!string.IsNullOrEmpty(Nombre))
            {
                query = query.Where(e => e.Nombre.Contains(Nombre) || e.Descripcion.Contains(Nombre));
            }
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            _db.Pelicula.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string Nombre)
        {
            bool result = _db.Pelicula.Any(x => x.Nombre.ToLower().Trim() == Nombre.ToLower().Trim());
            return result;
        }

        public bool ExistePelicula(int id)
        {
            bool result = _db.Pelicula.Any(x => x.Id == id);
            return result;
        }

        public Pelicula GetPelicula(int PeliculaId)
        {
            var pelicula = _db.Pelicula.FirstOrDefault(x => x.Id == PeliculaId);
            return pelicula;
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            var peliculas = _db.Pelicula.OrderBy(x => x.Nombre).ToList();
            return peliculas;
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int CatgId)
        {
            return _db.Pelicula.Include(c => c.Categoria).Where(x => x.categoriaId == CatgId).ToList();
        }

        public bool Guardar()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
