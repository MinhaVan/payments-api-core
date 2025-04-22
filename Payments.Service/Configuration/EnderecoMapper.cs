using AutoMapper;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

public class EnderecoMapper : Profile
{
    public EnderecoMapper()
    {
        CreateMap<EnderecoAdicionarViewModel, Endereco>().ReverseMap();
        CreateMap<EnderecoAtualizarViewModel, Endereco>().ReverseMap();
        CreateMap<EnderecoViewModel, Endereco>().ReverseMap();
    }
}