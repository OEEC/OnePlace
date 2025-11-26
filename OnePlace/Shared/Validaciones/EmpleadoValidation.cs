using FluentValidation;
using OnePlace.Shared.DTOs.Modelos;

namespace OnePlace.Shared.Validaciones
{
    public class EmpleadoValidation : AbstractValidator<EmpleadoPostDTO>
    {
        public EmpleadoValidation()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.ApellidoPat)
                .NotEmpty().WithMessage("El apellido paterno es obligatorio.")
                .MaximumLength(100).WithMessage("El apellido paterno no puede exceder los 100 caracteres.");

            RuleFor(x => x.ApellidoMat)
                .MaximumLength(100).WithMessage("El apellido materno no puede exceder los 100 caracteres.");

            RuleFor(x=>x.NoEmpleado)
                .NotNull().WithMessage("El número de empleado es obligatorio.")
                .NotEmpty().WithMessage("El número de empleado es obligatorio.")
                .MaximumLength(50).WithMessage("El número de empleado no puede exceder los 50 caracteres.");

            RuleFor(x => x.IdDepartamento)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un departamento")
                .NotNull()
                .WithMessage("El departamento es obligatorio.");

            RuleFor(x => x.IdEstacion)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar una estación")
                .NotNull()
                .WithMessage("La estación es obligatoria.");

            RuleFor(x => x.IdPuesto)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un puesto")
                .NotNull()
                .WithMessage("El puesto es obligatorio.");

            RuleFor(x => x.IdZona)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar una zona")
                .NotNull()
                .WithMessage("La zona es obligatoria.");

            RuleFor(x => x.Usuario)
                .NotEmpty()
                .WithMessage("El usuario no puede estar vacio")
                .MaximumLength(50)
                .WithMessage("El usuario no puede exceder los 50 caracteres.")
                .NotNull()
                .WithMessage("El usuario es obligatorio");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("La contraseña no puede estar vacia")
                .NotNull()
                .WithMessage("La contraseña es obligatoria");

        }
    }
}
