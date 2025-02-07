namespace Core.Entities;

public class Pedido : BaseEntity
{
    public int ClienteId { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataPedido { get; set; }
    
    // Navigation property
    public virtual Cliente Cliente { get; set; } = default!;
}

