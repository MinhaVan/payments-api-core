using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

public class PaginadoMapper : Profile
{
    public PaginadoMapper()
    {
        #region Paginado
        CreateMap(typeof(Paginado<>), typeof(PaginadoViewModel<>)).ReverseMap();
        #endregion
    }
}