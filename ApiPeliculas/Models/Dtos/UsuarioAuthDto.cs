using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Dtos
{
    public class UsuarioAuthDto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Este campo es obligatorio")]
        public string User { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(10,MinimumLength =4,ErrorMessage ="longitud debe estar entre 4 a 10 caracteres")]
        public string Password { get; set; }

    }
}
