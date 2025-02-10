using Core.DTOS.Cliente;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers;

/// <summary>
/// Handles CRUD operations for clientes (clients).
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientesController"/> class.
    /// </summary>
    /// <param name="clienteService">Service for handling cliente-related operations.</param>
    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Retrieves a list of all clientes.
    /// </summary>
    /// <returns>
    /// - 200 OK: Returns a list of clientes.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
    {
        var clientes = await _clienteService.GetAllAsync();
        return Ok(clientes);
    }

    /// <summary>
    /// Retrieves a specific cliente by ID.
    /// </summary>
    /// <param name="id">The ID of the cliente to retrieve.</param>
    /// <returns>
    /// - 200 OK: Returns the requested cliente.
    /// - 404 Not Found: If the cliente with the specified ID does not exist.
    /// </returns>

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetCliente(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente == null) return NotFound();
        
        return Ok(cliente);
    }

    /// <summary>
    /// Creates a new cliente.
    /// </summary>
    /// <param name="createClienteDto">The data for the new cliente.</param>
    /// <returns>
    /// - 201 Created: Returns the newly created cliente and the location of the resource.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> CreateCliente(CreateClienteDto createClienteDto)
    {
        var cliente = await _clienteService.CreateAsync(createClienteDto);
        return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
    }

    /// <summary>
    /// Updates an existing cliente.
    /// </summary>
    /// <param name="id">The ID of the cliente to update.</param>
    /// <param name="updateClienteDto">The updated data for the cliente.</param>
    /// <returns>
    /// - 204 No Content: If the update is successful.
    /// </returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCliente(int id, UpdateClienteDto updateClienteDto)
    {
        await _clienteService.UpdateAsync(id, updateClienteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a specific cliente by ID.
    /// </summary>
    /// <param name="id">The ID of the cliente to delete.</param>
    /// <returns>
    /// - 204 No Content: If the deletion is successful.
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCliente(int id)
    {
        await _clienteService.DeleteAsync(id);
        return NoContent();
    }
}