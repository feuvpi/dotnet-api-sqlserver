using Core.DTOS.Pedido;
using FluentValidation;

namespace Service.Validators;

public class CreatePedidoValidator : AbstractValidator<CreatePedidoDto>
{
    public CreatePedidoValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0);
            
        RuleFor(x => x.ValorTotal)
            .GreaterThan(0)
            .WithMessage("O valor total do pedido não pode ser negativo ou zero.");
    }
}

