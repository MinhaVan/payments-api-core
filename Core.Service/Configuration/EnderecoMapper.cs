using AutoMapper;
using Core.Domain.Models;

namespace Core.Service.Configurations;

public class EnderecoMapper : Profile
{
    public EnderecoMapper()
    {
        CreateMap<EnderecoAdicionarViewModel, Endereco>().ReverseMap();
        CreateMap<EnderecoAtualizarViewModel, Endereco>().ReverseMap();
        CreateMap<EnderecoViewModel, Endereco>().ReverseMap();
    }
}