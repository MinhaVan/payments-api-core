using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

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