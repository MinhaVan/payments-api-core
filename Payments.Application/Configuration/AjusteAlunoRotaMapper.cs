using AutoMapper;
using Payments.Domain.Models;
using Payments.Domain.ViewModels.Rota;

namespace Payments.Application.Configurations;

public class AjusteAlunoRotaMapper : Profile
{
    public AjusteAlunoRotaMapper()
    {
        #region RotaAjusteEndereco
        CreateMap<RotaAjusteEnderecoViewModel, AjusteAlunoRota>().ReverseMap();
        #endregion
    }
}