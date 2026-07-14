using DogTrainerTestTask.Dto;
using DogTrainerTestTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogTrainerTestTask.Controllers;

[ApiController]
[Route("api/litters/")]
public class LittersController(ILittersService littersService) : ControllerBase
{
    [HttpPost("{litterId}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetLittersAsync(
        [FromHeader(Name = "X-Breeder-Id")] long breederId,
        [FromRoute] long litterId
        )
    {
        await littersService.PublishLitterAsync(breederId, litterId);
        return NoContent();
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResultDto<LitterDto>),StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLittersAsync(
        [FromHeader(Name = "X-Breeder-Id")] long breederId,
        [FromQuery] GetLittersRequestDto getLittersRequestDto)
    {
        var result = await littersService.GetLittersByBreederIdAsync(breederId, getLittersRequestDto);
        return Ok(result);
    }
}