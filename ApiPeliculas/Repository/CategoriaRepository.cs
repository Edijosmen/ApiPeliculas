using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AplicationsDbContext _db;
        public CategoriaRepository(AplicationsDbContext db)
        {
            _db = db;
        }
        public bool ActualizarCategoria(Categoria categoria)
        {
            _db.Categoria.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _db.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            _db.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            bool result = _db.Categoria.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return result;
        }

        public bool ExisteCategoria(int id)
        {

            return _db.Categoria.Any(c => c.Id == id);
        }

        public Categoria GetCategoria(int CategoriaId)
        {
            return _db.Categoria.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> GetCategorias()   
        {
            return _db.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _db.SaveChanges() >= 0 ?  true: false;
        }
    }
}
