using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Payments.Domain.Interfaces.Repository;
using Payments.Domain.Enums;
using Payments.Domain.Models;
using Payments.Domain.ViewModels;
using Payments.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Payments.Application.Implementations;

public class AlunoService : IAlunoService
{
    private readonly IMapper _mapper;
    private readonly IBaseRepository<Aluno> _alunoRepository;
    private readonly IBaseRepository<AlunoRota> _AlunoRotaRepository;
    private readonly IBaseRepository<Rota> _rotaRepository;
    private readonly IUserContext _userContext;
    public AlunoService(
        IBaseRepository<Aluno> AlunoRepository,
        IBaseRepository<Rota> rotaRepository,
        IBaseRepository<AlunoRota> AlunoRotaRepository,
        IUserContext userContext,
        IMapper map)
    {
        _userContext = userContext;
        _mapper = map;
        _rotaRepository = rotaRepository;
        _AlunoRotaRepository = AlunoRotaRepository;
        _alunoRepository = AlunoRepository;
    }

    public async Task AdicionarAsync(int responsavelId, AlunoAdicionarViewModel AlunoVm)
    {
        var aluno = _mapper.Map<Aluno>(AlunoVm);
        aluno.ResponsavelId = responsavelId;
        aluno.Status = StatusEntityEnum.Ativo;
        aluno.EmpresaId = _userContext.Empresa;

        await _alunoRepository.AdicionarAsync(aluno);
    }

    public async Task AtualizarAsync(int responsavelId, AlunoViewModel AlunoVm)
    {
        var aluno = await _alunoRepository.ObterPorIdAsync(AlunoVm.Id);

        aluno.ResponsavelId = responsavelId;
        aluno.CPF = AlunoVm.CPF;
        aluno.PrimeiroNome = AlunoVm.PrimeiroNome;
        aluno.UltimoNome = AlunoVm.UltimoNome;
        aluno.Contato = AlunoVm.Contato;
        aluno.Email = AlunoVm.Email;
        aluno.EnderecoPartidaId = AlunoVm.EnderecoPartidaId;
        aluno.EnderecoRetornoId = AlunoVm.EnderecoRetornoId;
        aluno.EnderecoDestinoId = AlunoVm.EnderecoDestinoId;

        await _alunoRepository.AtualizarAsync(aluno);
    }

    public async Task DeletarAsync(int responsavelId, int AlunoId)
    {
        var Alunos = await _alunoRepository.BuscarAsync(x => x.ResponsavelId == responsavelId && x.Id == AlunoId);
        if (Alunos is null || !Alunos.Any())
            throw new Exception("Nenhum aluno encontrado para o usuário!");

        var aluno = Alunos.First();
        aluno.Status = StatusEntityEnum.Deletado;
        await _alunoRepository.AtualizarAsync(aluno);
    }

    public async Task<IList<AlunoViewModel>> ObterTodos(int responsavelId)
    {
        var Alunos = await _alunoRepository.BuscarAsync(x => x.ResponsavelId == responsavelId && x.Status == StatusEntityEnum.Ativo);
        return _mapper.Map<List<AlunoViewModel>>(Alunos);
    }

    public async Task<List<AlunoViewModel>> ObterAlunosPorFiltro(int rotaId, string filtro)
    {
        filtro = filtro.Trim().ToLower();

        // Buscar todos os alunos que correspondem ao filtro
        var alunos = await _alunoRepository.BuscarAsync(
            x => (EF.Functions.ILike(x.PrimeiroNome, $"%{filtro}%") ||
                EF.Functions.ILike(x.UltimoNome, $"%{filtro}%") ||
                EF.Functions.ILike(x.PrimeiroNome + " " + x.UltimoNome, $"%{filtro}%") ||
                EF.Functions.ILike(x.Contato, $"%{filtro}%") ||
                EF.Functions.ILike(x.Email, $"%{filtro}%") ||
                EF.Functions.ILike(x.CPF, $"%{filtro}%")) &&
                x.Status == StatusEntityEnum.Ativo &&
                x.EmpresaId == _userContext.Empresa,
            x => x.EnderecoPartida, x => x.EnderecoDestino, x => x.EnderecoRetorno, x => x.AlunoRotas
        );

        // Buscar IDs dos alunos que já estão cadastrados na rota
        var alunosNaRota = await _AlunoRotaRepository.BuscarAsync(
            x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo
        );

        var alunosNaRotaIds = alunosNaRota.Select(x => x.AlunoId).ToList();

        // Filtrar alunos que NÃO estão na rota
        var alunosForaDaRota = alunos.Where(x => !alunosNaRotaIds.Contains(x.Id)).ToList();

        return _mapper.Map<List<AlunoViewModel>>(alunosForaDaRota);
    }

    public async Task<IList<AlunoViewModel>> ObterAluno(int responsavelId, int AlunoId)
    {
        var Alunos = await _alunoRepository.BuscarAsync(x => x.ResponsavelId == responsavelId && x.Id == AlunoId);
        return _mapper.Map<List<AlunoViewModel>>(Alunos);
    }

    public async Task VincularRotaAsync(int rotaId, int AlunoId)
    {
        if (rotaId < 1 || AlunoId < 1)
            return;

        var AlunoRotas = await _AlunoRotaRepository.BuscarAsync(x =>
            x.AlunoId == AlunoId &&
            x.RotaId == rotaId &&
            x.Status != StatusEntityEnum.Ativo);

        var rotaExistente = await _rotaRepository.ObterPorIdAsync(rotaId);
        var AlunoExistente = await _alunoRepository.ObterPorIdAsync(AlunoId);

        if (rotaExistente is null)
        {
            throw new InvalidOperationException("A rota especificado não existe.");
        }

        if (AlunoExistente == null)
        {
            throw new InvalidOperationException("O aluno especificado não existe.");
        }

        if (AlunoRotas is null || !AlunoRotas.Any())
        {
            var AlunoRota = new AlunoRota
            {
                AlunoId = AlunoId,
                RotaId = rotaId
            };

            await _AlunoRotaRepository.AdicionarAsync(AlunoRota);
        }
        else
        {
            var AlunoRota = AlunoRotas.First();
            AlunoRota.Status = StatusEntityEnum.Ativo;
            await _AlunoRotaRepository.AtualizarAsync(AlunoRota);
        }
    }

    public async Task DesvincularRotaAsync(int rotaId, int AlunoId)
    {
        if (rotaId < 1 || AlunoId < 1)
            return;

        var AlunoRotas = await _AlunoRotaRepository.BuscarAsync(x =>
            x.AlunoId == AlunoId &&
            x.RotaId == rotaId);

        if (AlunoRotas is null || !AlunoRotas.Any())
        {
            throw new Exception("Nenhuma rota encontrada!");
        }

        var AlunoRota = AlunoRotas.First();
        AlunoRota.Status = StatusEntityEnum.Deletado;

        await _AlunoRotaRepository.AtualizarAsync(AlunoRota);
    }
}