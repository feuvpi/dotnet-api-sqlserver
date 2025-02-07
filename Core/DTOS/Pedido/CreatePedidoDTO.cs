namespace Core.DTOS.Pedido;

public record CreatePedidoDto(
    int ClienteId,
    decimal ValorTotal);