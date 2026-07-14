using DogTrainerTestTask.Dto;
using DogTrainerTestTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogTrainerTestTask.Controllers;

[ApiController]
[Route("api/litters/")]
public class LittersController(ILittersService littersService) : ControllerBase
{
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