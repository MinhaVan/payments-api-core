using System.Collections.Generic;

namespace Payments.Domain.ViewModels;

public class PermissaoViewModel
{
    public int UsuarioId { get; set; }
    public List<int> PermissaoId { get; set; }
}