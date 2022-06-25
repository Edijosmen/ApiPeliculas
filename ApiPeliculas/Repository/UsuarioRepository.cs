using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AplicationsDbContext _db;

        public UsuarioRepository(AplicationsDbContext db)
        {
            _db = db;
        }
        public bool ExisteUsuario(string usuario)
        {
            if (_db.Usuario.Any(x => x.User == usuario))
            {
                return true;
            }
            return false;
        }

        public Usuario GetUsuario(int UsuarioId)
        {
            return _db.Usuario.FirstOrDefault(u => u.Id == UsuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            var usuarios = _db.Usuario.OrderBy(u => u.User).ToList();
            return usuarios;

        }

        public bool Guardar()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public Usuario Login(string usuario, string password)
        {
            var user = _db.Usuario.FirstOrDefault(u => u.User == usuario);

            if (user == null)
            {
                return null;
            }

            if (!VerificaPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public Usuario RegistroUsuario(Usuario usuario, string password)
        {
            byte[] passwordHash, passwordSalt;
            CrearPasswordHash(password, out passwordHash, out passwordSalt);
            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;
            _db.Usuario.Add(usuario);
            Guardar();
            return usuario;
        }

        private bool VerificaPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                //var DATO = hashComputado.SequenceEqual(passwordHash);
                //return DATO;
                Console.WriteLine(hashComputado);
                for (int i = 0; i < hashComputado.Length; i++)
                {
                    if (hashComputado[i] != passwordHash[i]) return false;
                }
            }
            return true;

        }
        private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));


            }
        }
    }
}
