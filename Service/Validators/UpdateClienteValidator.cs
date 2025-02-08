using Core.DTOS.Cliente;
using FluentValidation;

namespace Service.Validators;

public class UpdateClienteValidator : AbstractValidator<UpdateClienteDto>
{
    public UpdateClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome não pode ter mais que 100 caracteres");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(100).WithMessage("Email não pode ter mais que 100 caracteres");
    }
}

