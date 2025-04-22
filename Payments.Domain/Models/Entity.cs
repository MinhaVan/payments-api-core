using System;
using Payments.Domain.Enums;

namespace Payments.Domain.Models;

public abstract class Entity
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    public StatusEntityEnum Status { get; set; }
}