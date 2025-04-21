using AutoMapper;
using Core.Domain.ViewModels;
using Core.Domain.Models;

namespace Core.Service.Configurations;

public class TokenMapper : Profile
{
    public TokenMapper()
    {
        CreateMap<TokenViewModel, Token>().ReverseMap();
    }
}