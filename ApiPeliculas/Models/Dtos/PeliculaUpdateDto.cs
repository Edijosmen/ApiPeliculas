using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ApiPeliculas.Models.Pelicula;

namespace ApiPeliculas.Models.Dtos
{
    public class PeliculaUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombre es Obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Ruta de imagen es Obligatorio")]
        public string RutaImagen { get; set; }
        [Required(ErrorMessage = "Descripcion es Obligatorio")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Duracion es Obligatorio")]
        public string Duracion { get; set; }

        public TipoClasificaion Clacificacion { get; set; }
        public int categoriaId { get; set; }
    }
}
