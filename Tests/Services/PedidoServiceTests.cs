using AutoMapper;
using Core.DTOS.Pedido;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentAssertions;
using Moq;
using Service;

namespace Tests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PedidoService _sut;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new PedidoService(_pedidoRepositoryMock.Object, _clienteRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingCliente_ThrowsBusinessRuleException()
    {
        // -- Arrange
        var createPedidoDto = new CreatePedidoDto(1, 100.0m);
        _clienteRepositoryMock.Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(false);

        // -- Act
        var act = () => _sut.CreateAsync(createPedidoDto);

        // -- Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Cliente não encontrado.");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsPedidoDto()
    {
        // -- Arrange
        var createPedidoDto = new CreatePedidoDto(1, 100.0m);
        var pedido = new Pedido { Id = 1, ClienteId = 1, ValorTotal = 100.0m };
        var pedidoDto = new PedidoDto(1, 1, 100.0m, DateTime.UtcNow);

        _clienteRepositoryMock.Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);
        _mapperMock.Setup(x => x.Map<Pedido>(createPedidoDto))
            .Returns(pedido);
        _pedidoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(pedido);
        _mapperMock.Setup(x => x.Map<PedidoDto>(pedido))
            .Returns(pedidoDto);

        // -- Act
        var result = await _sut.CreateAsync(createPedidoDto);

        // -- Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(pedidoDto);
    }

    [Fact]
    public async Task GetByClienteIdAsync_ReturnsCorrectPedidos()
    {
        // -- Arrange
        var clienteId = 1;
        var pedidos = new List<Pedido> 
        { 
            new() { Id = 1, ClienteId = clienteId, ValorTotal = 100.0m },
            new() { Id = 2, ClienteId = clienteId, ValorTotal = 200.0m }
        };
        var pedidosDto = pedidos.Select(p => new PedidoDto(p.Id, p.ClienteId, p.ValorTotal, DateTime.UtcNow));

        _pedidoRepositoryMock.Setup(x => x.GetPedidosByClienteIdAsync(clienteId))
            .ReturnsAsync(pedidos);
        _mapperMock.Setup(x => x.Map<IEnumerable<PedidoDto>>(pedidos))
            .Returns(pedidosDto);

        // -- Act
        var result = await _sut.GetByClienteIdAsync(clienteId);

        // -- Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(pedidosDto);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingPedido_ThrowsNotFoundException()
    {
        // -- Arrange
        var id = 1;
        var updatePedidoDto = new UpdatePedidoDto(150.0m);
        
        _pedidoRepositoryMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Pedido?)null);

        // -- Act
        var act = () => _sut.UpdateAsync(id, updatePedidoDto);

        // -- Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WithExistingPedido_DeletesSuccessfully()
    {
        // -- Arrange
        var pedido = new Pedido { Id = 1, ClienteId = 1, ValorTotal = 100.0m };
        
        _pedidoRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(pedido);

        // -- Act
        await _sut.DeleteAsync(1);

        // -- Assert
        _pedidoRepositoryMock.Verify(x => x.DeleteAsync(pedido), Times.Once);
    }
}