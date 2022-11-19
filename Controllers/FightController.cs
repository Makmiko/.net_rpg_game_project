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
        return Ok(await _fightService.DefaultAttack(request));
    }

    [HttpPost("SkillAttack")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackDto request)
    {
        return Ok(await _fightService.SkillAttack(request));
    }

    [HttpPost("InitFight")]
    public async Task<ActionResult<ServiceResponse<FightResultDto>>> InitFight(FightRequestDto request)
    {
        return Ok(await _fightService.InitFight(request));
    }
}