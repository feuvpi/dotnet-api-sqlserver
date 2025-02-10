using AutoMapper;
using Core.DTOS.Cliente;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentAssertions;
using Moq;
using Service;

namespace Tests.Services;


public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ClienteService _sut;

    public ClienteServiceTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new ClienteService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsClienteDto()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nome = "Test", Email = "test@test.com" };
        var clienteDto = new ClienteDto(1, "Test", "test@test.com", DateTime.UtcNow);

        _repositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(cliente);
        _mapperMock.Setup(x => x.Map<ClienteDto>(cliente))
            .Returns(clienteDto);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(clienteDto);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingPedidos_ThrowsBusinessRuleException()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nome = "Test", Email = "test@test.com" };
        
        _repositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(cliente);
        _repositoryMock.Setup(x => x.HasPedidosAsync(1))
            .ReturnsAsync(true);

        // Act
        var act = () => _sut.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Não é possível excluir um cliente que possui pedidos associados.");
    }
}