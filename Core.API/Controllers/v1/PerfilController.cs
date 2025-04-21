using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class PerfilController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Obter()
    {
        return Success(await GetPerfil());
    }

    private async Task<Dictionary<string, int>> GetPerfil()
    {
        return await Task.Run(() =>
        {
            var enumValues = new Dictionary<string, int>();
            foreach (var value in Enum.GetValues(typeof(PerfilEnum)))
            {
                enumValues.Add(value.ToString(), (int)value);
            }

            return enumValues;
        });
    }
}