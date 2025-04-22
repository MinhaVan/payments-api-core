using System.Collections.Generic;

namespace Payments.Domain.Models;

public class Plano : Entity
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public int EmpresaId { get; set; }
    //
    public virtual IList<Assinatura> Assinaturas { get; set; } = new List<Assinatura>();
    public virtual IList<Usuario> Usuarios { get; set; }
    public virtual Empresa Empresa { get; set; }
}