

using AutoMapper;
using Core.DTOS.Cliente;
using Core.DTOS.Pedido;
using Core.Entities;

namespace Service.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Cliente, ClienteDto>();
        CreateMap<CreateClienteDto, Cliente>();
        CreateMap<UpdateClienteDto, Cliente>();
        
        CreateMap<Pedido, PedidoDto>();
        CreateMap<CreatePedidoDto, Pedido>();
        CreateMap<UpdatePedidoDto, Pedido>();
    }
}

