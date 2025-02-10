using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.Property(p => p.ValorTotal)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(p => p.DataPedido)
            .IsRequired();
    }
}