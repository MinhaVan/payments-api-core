using System.Collections.Generic;

namespace Payments.Domain.ViewModels;

public class PaginadoViewModel<T>
{
    public PaginadoViewModel(int pagina, int tamanho, int quantidade, List<T> data)
    {
        Data = data;
        Quantidade = quantidade;
        Tamanho = tamanho;
        Pagina = pagina;
    }

    public int Pagina { get; set; }
    public int Tamanho { get; set; }
    public int Quantidade { get; set; }
    public List<T> Data { get; set; }
}