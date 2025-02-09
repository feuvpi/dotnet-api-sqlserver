using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<bool> HasPedidosAsync(int id)
    {
        return await _context.Pedidos.AnyAsync(p => p.ClienteId == id);
    }
}