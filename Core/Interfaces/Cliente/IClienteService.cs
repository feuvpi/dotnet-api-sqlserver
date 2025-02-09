using Core.DTOS.Cliente;

namespace Core.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> GetAllAsync();
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<ClienteDto> CreateAsync(CreateClienteDto createClienteDto);
    Task UpdateAsync(int id, UpdateClienteDto updateClienteDto);
    Task DeleteAsync(int id);
}