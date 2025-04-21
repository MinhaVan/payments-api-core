using System.Collections.Generic;

namespace Core.Domain.ViewModels;

public class PermissaoViewModel
{
    public int UsuarioId { get; set; }
    public List<int> PermissaoId { get; set; }
}