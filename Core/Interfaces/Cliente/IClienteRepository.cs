using Core.Entities;

namespace Core.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>
{
    Task<bool> HasPedidosAsync(int id);
}



