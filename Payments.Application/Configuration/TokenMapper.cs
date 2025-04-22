using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

public class TokenMapper : Profile
{
    public TokenMapper()
    {
        CreateMap<TokenViewModel, Token>().ReverseMap();
    }
}