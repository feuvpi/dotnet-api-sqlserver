using AutoMapper;
using Core.DTOS.Pedido;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Service;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IMapper _mapper;

    public PedidoService(
        IPedidoRepository repository,
        IClienteRepository clienteRepository,
        IMapper mapper)
    {
        _repository = repository;
        _clienteRepository = clienteRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PedidoDto>> GetAllAsync()
    {
        var pedidos = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PedidoDto>>(pedidos);
    }

    public async Task<PedidoDto?> GetByIdAsync(int id)
    {
        var pedido = await _repository.GetByIdAsync(id);
        return pedido == null ? null : _mapper.Map<PedidoDto>(pedido);
    }

    public async Task<IEnumerable<PedidoDto>> GetByClienteIdAsync(int clienteId)
    {
        var pedidos = await _repository.GetPedidosByClienteIdAsync(clienteId);
        return _mapper.Map<IEnumerable<PedidoDto>>(pedidos);
    }

    public async Task<PedidoDto> CreateAsync(CreatePedidoDto createPedidoDto)
    {
        
        if(createPedidoDto.ValorTotal < 0)
            throw new BusinessRuleException("O valor do pedido não pode ser negativo.");
        
        if (!await _clienteRepository.ExistsAsync(createPedidoDto.ClienteId))
            throw new BusinessRuleException("Cliente não encontrado.");
        
        var pedido = _mapper.Map<Pedido>(createPedidoDto);
        pedido.DataPedido = DateTime.UtcNow;
        
        await _repository.AddAsync(pedido);
        return _mapper.Map<PedidoDto>(pedido);
    }

    public async Task UpdateAsync(int id, UpdatePedidoDto updatePedidoDto)
    {
        if(updatePedidoDto.ValorTotal < 0)
            throw new BusinessRuleException("O valor total do pedido nõo pode ser negativo.");
        var pedido = await _repository.GetByIdAsync(id);
        if (pedido == null)
            throw new NotFoundException(nameof(Pedido), id);
        
        

        _mapper.Map(updatePedidoDto, pedido);
        await _repository.UpdateAsync(pedido);
    }

    public async Task DeleteAsync(int id)
    {
        var pedido = await _repository.GetByIdAsync(id);
        if (pedido == null)
            throw new NotFoundException(nameof(Pedido), id);

        await _repository.DeleteAsync(pedido);
    }
}