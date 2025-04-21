using AutoMapper;
using Core.Domain.ViewModels;
using Core.Domain.Models;

namespace Core.Service.Configurations;

public class MotoristaMapper : Profile
{
    public MotoristaMapper()
    {
        #region Motorista
        CreateMap<MotoristaNovoViewModel, UsuarioNovoViewModel>().ReverseMap();
        CreateMap<MotoristaNovoViewModel, Motorista>().ReverseMap();
        CreateMap<UsuarioViewModel, MotoristaViewModel>().ReverseMap();
        CreateMap<Motorista, MotoristaViewModel>().ReverseMap();
        CreateMap<Usuario, MotoristaViewModel>().ReverseMap();
        #endregion
    }
}