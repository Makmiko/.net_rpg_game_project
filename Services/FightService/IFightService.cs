using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services;

public interface IFightService
{
    public Task<ServiceResponse<AttackResultDto>> DefaultAttack(DefaultAttackDto request);
    public Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request);
    public Task<ServiceResponse<FightResultDto>> InitFight(FightRequestDto request);
}