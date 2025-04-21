using System.Collections.Generic;

namespace Core.Domain.Models;
public class Empresa : Entity
{
    public string CNPJ { get; set; }
    public string NomeExibicao { get; set; }
    public string Descricao { get; set; }
    public string NomeFantasia { get; set; }
    public string RazaoSocial { get; set; }
    public string Apelido { get; set; }
    //
    public virtual List<Endereco> Enderecos { get; set; }
    public virtual List<Usuario> Usuarios { get; set; }
    public virtual List<Aluno> Alunos { get; set; }
    public virtual List<Veiculo> Veiculos { get; set; }
    public virtual List<Plano> Planos { get; set; }
}