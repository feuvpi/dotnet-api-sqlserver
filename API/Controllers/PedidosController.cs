using AutoMapper;
using Core.DTOS.Pedido;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoRepository _repository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IMapper _mapper;

    public PedidosController(
        IPedidoRepository repository,
        IClienteRepository clienteRepository,
        IMapper mapper)
    {
        _repository = repository;
        _clienteRepository = clienteRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoDto>>> GetPedidos()
    {
        var pedidos = await _repository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<PedidoDto>>(pedidos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PedidoDto>> GetPedido(int id)
    {
        var pedido = await _repository.GetByIdAsync(id);
        if (pedido == null) return NotFound();
        
        return Ok(_mapper.Map<PedidoDto>(pedido));
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<PedidoDto>>> GetPedidosByCliente(int clienteId)
    {
        var pedidos = await _repository.GetPedidosByClienteIdAsync(clienteId);
        return Ok(_mapper.Map<IEnumerable<PedidoDto>>(pedidos));
    }

    [HttpPost]
    public async Task<ActionResult<PedidoDto>> CreatePedido(CreatePedidoDto createPedidoDto)
    {
        if (!await _clienteRepository.ExistsAsync(createPedidoDto.ClienteId))
            throw new BusinessRuleException("Cliente não encontrado.");

        var pedido = _mapper.Map<Pedido>(createPedidoDto);
        pedido.DataPedido = DateTime.UtcNow;
        
        await _repository.AddAsync(pedido);
        
        return CreatedAtAction(
            nameof(GetPedido),
            new { id = pedido.Id },
            _mapper.Map<PedidoDto>(pedido));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePedido(int id, UpdatePedidoDto updatePedidoDto)
    {
        var pedido = await _repository.GetByIdAsync(id);
        if (pedido == null) return NotFound();

        _mapper.Map(updatePedidoDto, pedido);
        await _repository.UpdateAsync(pedido);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePedido(int id)
    {
        var pedido = await _repository.GetByIdAsync(id);
        if (pedido == null) return NotFound();

        await _repository.DeleteAsync(pedido);
        return NoContent();
    }
}