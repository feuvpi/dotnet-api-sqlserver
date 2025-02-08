using AutoMapper;
using Core.DTOS.Cliente;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IMapper _mapper;

    public ClienteService(IClienteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var clientes = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        return cliente == null ? null : _mapper.Map<ClienteDto>(cliente);
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto createClienteDto)
    {
        var cliente = _mapper.Map<Cliente>(createClienteDto);
        cliente.DataCadastro = DateTime.UtcNow;
        
        await _repository.AddAsync(cliente);
        return _mapper.Map<ClienteDto>(cliente);
    }

    public async Task UpdateAsync(int id, UpdateClienteDto updateClienteDto)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null) 
            throw new NotFoundException(nameof(Cliente), id);

        _mapper.Map(updateClienteDto, cliente);
        await _repository.UpdateAsync(cliente);
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null)
            throw new NotFoundException(nameof(Cliente), id);

        if (await _repository.HasPedidosAsync(id))
            throw new BusinessRuleException("Não é possível excluir um cliente que possui pedidos associados.");

        await _repository.DeleteAsync(cliente);
    }
}
