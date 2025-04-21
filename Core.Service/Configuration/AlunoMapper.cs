using AutoMapper;
using Core.Domain.ViewModels;
using Core.Domain.Models;

namespace Core.Service.Configurations;

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