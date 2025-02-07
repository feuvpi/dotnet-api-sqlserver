namespace Core.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime DataCadastro { get; set; }
    
    // Navigation property
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}