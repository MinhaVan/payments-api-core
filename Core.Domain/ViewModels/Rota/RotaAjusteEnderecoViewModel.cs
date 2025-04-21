using System;
using Core.Domain.Models;

namespace Core.Domain.ViewModels.Rota;
public class RotaAjusteEnderecoViewModel
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public int RotaId { get; set; }
    public int NovoEnderecoId { get; set; }
    public DateTime Data { get; set; }
    //
    public EnderecoViewModel EnderecoDestino { get; set; }
    public EnderecoViewModel EnderecoPartida { get; set; }
    public EnderecoViewModel EnderecoRetorno { get; set; }
}