using Core.DTOS.Pedido;

namespace Core.Interfaces;

public interface IPedidoService
{
    Task<IEnumerable<PedidoDto>> GetAllAsync();
    Task<PedidoDto?> GetByIdAsync(int id);
    Task<IEnumerable<PedidoDto>> GetByClienteIdAsync(int clienteId);
    Task<PedidoDto> CreateAsync(CreatePedidoDto createPedidoDto);
    Task UpdateAsync(int id, UpdatePedidoDto updatePedidoDto);
    Task DeleteAsync(int id);
}