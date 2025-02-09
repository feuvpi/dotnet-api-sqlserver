using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class PedidoRepository : GenericRepository<Pedido>, IPedidoRepository
{
    public PedidoRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IReadOnlyList<Pedido>> GetPedidosByClienteIdAsync(int clienteId)
    {
        return await _context.Pedidos
            .Where(p => p.ClienteId == clienteId)
            .ToListAsync();
    }
}

