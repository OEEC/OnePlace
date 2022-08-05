using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnePlace.Shared.Helpers
{
    public class CustomPasswordValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //if (string.IsNullOrWhiteSpace(input))
            //{
            //    return new ValidationResult($"El campo contraseña es requerido",
            //    new[] { validationContext.MemberName });
            //}
            //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", ErrorMessage = "Password must be between 6 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]

            var input = value.ToString();

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,20}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                return new ValidationResult($"La contraseña debe contener al menos una letra minúscula.",
                new[] { validationContext.MemberName });
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                return new ValidationResult($"La contraseña debe contener al menos una letra mayúscula.",
                new[] { validationContext.MemberName });
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                return new ValidationResult($"La contraseña no debe tener menos de 8 ni más de 255 caracteres.",
                new[] { validationContext.MemberName });
            }
            else if (!hasNumber.IsMatch(input))
            {
                return new ValidationResult($"La contraseña debe contener al menos un valor numérico.",
                new[] { validationContext.MemberName });
            }

            else if (!hasSymbols.IsMatch(input))
            {
                return new ValidationResult($"La contraseña debe contener al menos un carácter de caso especial.",
                new[] { validationContext.MemberName });
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
