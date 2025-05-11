using System.Threading.Tasks;
using Payments.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Payments.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class CacheController : BaseController
{
    // private readonly IRedisRepository _redisRepository;

    // public CacheController(IRedisRepository redisRepository)
    // {
    //     _redisRepository = redisRepository;
    // }

    // [HttpDelete("{key}")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> Delete([FromRoute] string key)
    // {
    //     await _redisRepository.RemoveAsync(key);
    //     return Ok();
    // }
}