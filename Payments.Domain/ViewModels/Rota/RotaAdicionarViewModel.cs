using System;
using Payments.Domain.Enums;

namespace Payments.Domain.ViewModels;

public class RotaAdicionarViewModel
{
    public int? VeiculoId { get; set; }
    public string Nome { get; set; }
    public DiaSemanaEnum DiaSemana { get; set; }
    public TimeOnly Horario { get; set; }
    public TipoRotaEnum TipoRota { get; set; }
}