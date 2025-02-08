using Core.DTOS.Pedido;
using FluentValidation;

namespace Service.Validators;

public class UpdatePedidoValidator : AbstractValidator<UpdatePedidoDto>
{
    public UpdatePedidoValidator()
    {
        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).WithMessage("Valor total deve ser maior que zero")
            .NotNull().WithMessage("Valor total é obrigatório");
    }
}
