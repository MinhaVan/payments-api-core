using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Payments.Domain.Interfaces.Repository;
using Payments.Domain.Interfaces.Services;
using Payments.Domain.Models;
using Payments.Domain.ViewModels;

namespace Payments.Application.Implementations;

public class PlanoService : IPlanoService
{
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBaseRepository<Plano> _planoRepository;
    public PlanoService(IUserContext userContext, IMapper mapper, IBaseRepository<Plano> planoRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _planoRepository = planoRepository;
    }

    public async Task Adicionar(PlanoAdicionarViewModel planoVM)
    {
        var plano = _mapper.Map<Plano>(planoVM);
        plano.EmpresaId = _userContext.Empresa;
        plano.Status = Domain.Enums.StatusEntityEnum.Ativo;
        await _planoRepository.AdicionarAsync(plano);
    }

    public async Task Atualizar(PlanoViewModel planoVM)
    {
        var plano = await _planoRepository.ObterPorIdAsync(planoVM.Id);
        plano.Descricao = planoVM.Descricao;
        plano.Nome = planoVM.Nome;
        plano.Valor = planoVM.Valor;
        plano.Status = planoVM.Ativo ? Domain.Enums.StatusEntityEnum.Ativo : Domain.Enums.StatusEntityEnum.Deletado;

        await _planoRepository.AtualizarAsync(plano);
    }

    public async Task Deletar(int planoId)
    {
        var plano = await _planoRepository.ObterPorIdAsync(planoId);
        plano.Status = Domain.Enums.StatusEntityEnum.Deletado;

        await _planoRepository.AtualizarAsync(plano);
    }

    public async Task<List<PlanoViewModel>> Obter(int empresaId)
    {
        var planos = await _planoRepository.BuscarAsync(x =>
            x.EmpresaId == empresaId &&
            x.Status == Domain.Enums.StatusEntityEnum.Ativo);

        return _mapper.Map<List<PlanoViewModel>>(planos);
    }
}