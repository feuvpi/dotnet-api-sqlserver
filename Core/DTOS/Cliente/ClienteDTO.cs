namespace Core.DTOS.Cliente;

public record ClienteDto(
    int Id,
    string Nome,
    string Email,
    DateTime DataCadastro);