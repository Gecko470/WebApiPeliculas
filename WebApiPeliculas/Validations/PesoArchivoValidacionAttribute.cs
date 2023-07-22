using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.Validations
{
    public class PesoArchivoValidacionAttribute : ValidationAttribute
    {
        private readonly int pesoMaximo;

        public PesoArchivoValidacionAttribute(int pesoMaximo)
        {
            this.pesoMaximo = pesoMaximo;
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

            if (formFile.Length > pesoMaximo * 1024 * 1024)
            {
                return new ValidationResult($"El peso máximo de la imagen debe ser {pesoMaximo} MB..");
            }

            return ValidationResult.Success;
        }
    }
}
