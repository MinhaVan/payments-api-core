using AutoMapper;
using Core.Domain.ViewModels;
using Core.Domain.Models;
using Core.Domain.ApiModel;
using System;
using Core.Domain.Enums;
using Core.Domain.ViewModels.Assinatura;
using System.Linq;
using Core.Domain.ViewModels.Rota;

namespace Core.Service.Configurations;

public class AssinaturaMapper : Profile
{
    public AssinaturaMapper()
    {
        #region Assinatura
        CreateMap<CreditoViewModel, AssinaturaRequest>()
            .ForMember(x => x.Vencimento, opt => opt.MapFrom(z => DateTime.UtcNow))
            .ReverseMap();

        CreateMap<CartaoCreditoViewModel, CartaoCreditoRequest>().ReverseMap();
        CreateMap<InformacoesTitularCartaoViewModel, InformacoesTitularCartaoRequest>().ReverseMap();

        //
        CreateMap<Assinatura, AssinaturaViewModel>()
            .ForMember(x => x.Pagamentos, opt => opt.MapFrom(z => z.Pagamentos.OrderByDescending(x => x.DataCriacao)))
            .ReverseMap();

        CreateMap<Pagamento, PagamentoHistoricoViewModel>().ReverseMap();
        #endregion
    }
}