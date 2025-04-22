using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UsuarioDetalheViewModel, Usuario>().ReverseMap();

        CreateMap<UsuarioViewModel, Usuario>()
            .ForMember(dest => dest.Senha, opt => opt.MapFrom(src => src.Senha))
            .ReverseMap()
            .ForMember(dest => dest.Senha, opt => opt.MapFrom(src => (string?)null));

        CreateMap<UsuarioNovoViewModel, Usuario>().ReverseMap();
        CreateMap<UsuarioLoginViewModel, Usuario>().ReverseMap();
        CreateMap<UsuarioAtualizarViewModel, Usuario>().ReverseMap();
    }
}