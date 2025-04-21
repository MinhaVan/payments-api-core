using System;
using Core.Domain.Enums;

namespace Core.Domain.ViewModels;
public class MotoristaNovoViewModel : UsuarioNovoViewModel
{
    public string CNH { get; set; }
    public DateTime Vencimento { get; set; }
    public TipoCNHEnum TipoCNH { get; set; }
    public string Foto { get; set; }
}