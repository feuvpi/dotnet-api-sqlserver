using Core.Entities;

namespace Core.Interfaces;

public interface IPedidoRepository : IGenericRepository<Pedido>
{
    Task<IReadOnlyList<Pedido>> GetPedidosByClienteIdAsync(int clienteId);
}