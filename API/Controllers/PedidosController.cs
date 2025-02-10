using Core.DTOS.Pedido;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers;

/// <summary>
/// Handles CRUD operations for Pedidos.
/// </summary>

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PedidosController"/> class.
    /// </summary>
    /// <param name="pedidoService">Service for handling pedido-related operations.</param>
    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    /// <summary>
    /// Retrieves a list of all pedidos.
    /// </summary>
    /// <returns>
    /// - 200 OK: Returns a list of pedidos.
    /// </returns>=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoDto>>> GetPedidos()
    {
        var pedidos = await _pedidoService.GetAllAsync();
        return Ok(pedidos);
    }

    /// <summary>
    /// Retrieves a specific pedido by ID.
    /// </summary>
    /// <param name="id">The ID of the pedido to retrieve.</param>
    /// <returns>
    /// - 200 OK: Returns the requested pedido.
    /// - 404 Not Found: If the pedido with the specified ID does not exist.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PedidoDto>> GetPedido(int id)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);
        if (pedido == null) return NotFound();
        
        return Ok(pedido);
    }

    /// <summary>
    /// Retrieves a list of pedidos for a specific cliente.
    /// </summary>
    /// <param name="clienteId">The ID of the cliente to retrieve pedidos for.</param>
    /// <returns>
    /// - 200 OK: Returns a list of pedidos for the specified cliente.
    /// </returns>
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<PedidoDto>>> GetPedidosByCliente(int clienteId)
    {
        var pedidos = await _pedidoService.GetByClienteIdAsync(clienteId);
        return Ok(pedidos);
    }

    /// <summary>
    /// Creates a new pedido.
    /// </summary>
    /// <param name="createPedidoDto">The data for the new pedido.</param>
    /// <returns>
    /// - 201 Created: Returns the newly created pedido and the location of the resource.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<PedidoDto>> CreatePedido(CreatePedidoDto createPedidoDto)
    {
        var pedido = await _pedidoService.CreateAsync(createPedidoDto);
        return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
    }

    /// <summary>
    /// Updates an existing pedido.
    /// </summary>
    /// <param name="id">The ID of the pedido to update.</param>
    /// <param name="updatePedidoDto">The updated data for the pedido.</param>
    /// <returns>
    /// - 204 No Content: If the update is successful.
    /// </returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePedido(int id, UpdatePedidoDto updatePedidoDto)
    {
        await _pedidoService.UpdateAsync(id, updatePedidoDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a specific pedido by ID.
    /// </summary>
    /// <param name="id">The ID of the pedido to delete.</param>
    /// <returns>
    /// - 204 No Content: If the deletion is successful.
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePedido(int id)
    {
        await _pedidoService.DeleteAsync(id);
        return NoContent();
    }
}