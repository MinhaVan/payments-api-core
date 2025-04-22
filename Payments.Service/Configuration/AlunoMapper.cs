using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

public class AlunoMapper : Profile
{
    public AlunoMapper()
    {
        CreateMap<AlunoViewModel, Aluno>().ReverseMap();
        CreateMap<AlunoAdicionarViewModel, Aluno>().ReverseMap();
        CreateMap<AlunoRotaViewModel, AlunoRota>().ReverseMap();
        CreateMap<AlunoDetalheViewModel, Aluno>().ReverseMap();
    }
}