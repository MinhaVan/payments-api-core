namespace Core.Domain.ViewModels;

public class PlanoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public bool Ativo { get; set; }
}