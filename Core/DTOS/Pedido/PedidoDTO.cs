namespace Core.DTOS.Pedido;

public record PedidoDto(
    int Id,
    int ClienteId,
    decimal ValorTotal,
    DateTime DataPedido);
