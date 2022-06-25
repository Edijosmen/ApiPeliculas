using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Dtos
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="el campo es obligatorio")]
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
