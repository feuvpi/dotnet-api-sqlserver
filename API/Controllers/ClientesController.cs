using AutoMapper;
using Core.DTOS.Cliente;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepository _repository;
    private readonly IMapper _mapper;

    public ClientesController(IClienteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
    {
        var clientes = await _repository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ClienteDto>>(clientes));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetCliente(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null) return NotFound();
        
        return Ok(_mapper.Map<ClienteDto>(cliente));
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> CreateCliente(CreateClienteDto createClienteDto)
    {
        var cliente = _mapper.Map<Cliente>(createClienteDto);
        cliente.DataCadastro = DateTime.UtcNow;
        
        await _repository.AddAsync(cliente);
        
        return CreatedAtAction(
            nameof(GetCliente),
            new { id = cliente.Id },
            _mapper.Map<ClienteDto>(cliente));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCliente(int id, UpdateClienteDto updateClienteDto)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null) return NotFound();

        _mapper.Map(updateClienteDto, cliente);
        await _repository.UpdateAsync(cliente);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCliente(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null) return NotFound();

        if (await _repository.HasPedidosAsync(id))
            throw new BusinessRuleException("Não é possível excluir um cliente que possui pedidos associados.");

        await _repository.DeleteAsync(cliente);
        return NoContent();
    }
}