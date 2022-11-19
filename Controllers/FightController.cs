using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FightController : ControllerBase
{
    private readonly IFightService _fightService;

    public FightController(IFightService fightService)
    {
        _fightService = fightService;
    }

    [HttpPost("DefaultAttack")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> DefaultAttack(DefaultAttackDto request)
    {
        var response = await _fightService.DefaultAttack(request);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpPost("SkillAttack")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackDto request)
    {
        var response = await _fightService.SkillAttack(request);
        if (!response.Success) return BadRequest(response);
        return Ok(response);    }

    [HttpPost("InitFight")]
    public async Task<ActionResult<ServiceResponse<FightResultDto>>> InitFight(FightRequestDto request)
    {
        var response = await _fightService.InitFight(request);
        if (!response.Success) return BadRequest(response);
        return Ok(response);    }

    [HttpGet("Highscore")]
    public async Task<ActionResult<ServiceResponse<List<HighscoreDto>>>> GetHighscore()
    {
        var response = await _fightService.GetHighscore();
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}