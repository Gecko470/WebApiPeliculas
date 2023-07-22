using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.Validations
{
    public class TipoArchivoValidacion : ValidationAttribute
    {
        private readonly string[] tiposValidos;

        public TipoArchivoValidacion(string[] tiposValidos)
        {
            this.tiposValidos = tiposValidos;
        }
        public TipoArchivoValidacion(GrupoTiposArchivo tipoArchivo)
        {
            if (tipoArchivo == GrupoTiposArchivo.Imagen)
            {
                tiposValidos = new string[] { "image/jpg", "image/jpeg", "image/png", "image/gif" };
            }
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (!tiposValidos.Contains(formFile.ContentType.ToString()))
            {
                return new ValidationResult($"El tipo de archivo debe ser {string.Join(", ", tiposValidos)} ..");
            }

            return ValidationResult.Success;
        }
    }
}
