using System.Collections;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services;

public class FightService : IFightService
{
    private readonly DataContext _context;

    public FightService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<AttackResultDto>> DefaultAttack(DefaultAttackDto request)
    {
        var serviceResponse = new ServiceResponse<AttackResultDto>();
        try
        {
            var attacker = await _context.Characters
                .Include(chr => chr.Weapon)
                .FirstOrDefaultAsync(chr => chr.Id == request.AttackerId);
            var defender = await _context.Characters
                .FirstOrDefaultAsync(chr => chr.Id == request.DefenderId);
            if (attacker == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Attacker not found";
                return serviceResponse;
            }

            if (defender == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Defender not found";
                return serviceResponse;
            }

            var damage = InitDefaultAttack(attacker, defender);

            if (defender.Hitpoints <= 0)
            {
                serviceResponse.Message = $"{defender.Name} has been defeated";
            }

            await _context.SaveChangesAsync();
            serviceResponse.Data = new AttackResultDto()
            {
                Attacker = attacker.Name,
                Defender = defender.Name,
                AttackerHp = attacker.Hitpoints,
                DefenderHp = defender.Hitpoints,
                Damage = damage,
            };
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    private static int InitDefaultAttack(Character attacker, Character defender)
    {
        int damage = (attacker.Weapon?.Damage ?? 0) + new Random().Next(attacker.Strength);
        damage -= new Random().Next(defender.Defense);

        if (damage > 0)
        {
            defender.Hitpoints -= damage;
        }

        return damage;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
    {
        var serviceResponse = new ServiceResponse<AttackResultDto>();
        try
        {
            var attacker = await _context.Characters
                .Include(chr => chr.Skills)
                .FirstOrDefaultAsync(chr => chr.Id == request.AttackerId);
            var defender = await _context.Characters
                .FirstOrDefaultAsync(chr => chr.Id == request.DefenderId);
            if (attacker == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Attacker not found";
                return serviceResponse;
            }

            if (defender == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Defender not found";
                return serviceResponse;
            }

            var skill = attacker.Skills.FirstOrDefault(skill => skill.Id == request.SkillId);
            if (skill == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"{attacker.Name} does not know that skill";
                return serviceResponse;
            }

            var damage = InitSkillAttack(skill, attacker, defender);

            if (defender.Hitpoints <= 0)
            {
                serviceResponse.Message = $"{defender.Name} has been defeated";
            }

            await _context.SaveChangesAsync();
            serviceResponse.Data = new AttackResultDto()
            {
                Attacker = attacker.Name,
                Defender = defender.Name,
                AttackerHp = attacker.Hitpoints,
                DefenderHp = defender.Hitpoints,
                Damage = damage,
            };
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    private static int InitSkillAttack(Skill? skill, Character attacker, Character defender)
    {
        int damage = (skill?.Damage ?? 0) + new Random().Next(attacker.Intelligence);
        damage -= new Random().Next(defender.Defense);

        if (damage > 0)
        {
            defender.Hitpoints -= damage;
        }

        return damage;
    }

    public async Task<ServiceResponse<FightResultDto>> InitFight(FightRequestDto request)
    {
        var serviceResponse = new ServiceResponse<FightResultDto>()
        {
            Data = new FightResultDto(),
        };
        try
        {
            var characters = await _context.Characters
                .Include(chr => chr.Weapon)
                .Include(chr => chr.Skills)
                .Where(chr => request.CharacterIds.Contains(chr.Id)).ToListAsync();

            bool defeated = false;
            var initialHp = new Dictionary<string, int>();
            characters.ForEach(chr =>
            {
                initialHp.Add(chr.Name, chr.Hitpoints);
            });
            while (!defeated)
            {
                foreach (Character attacker in characters)
                {
                    var opponents = characters
                        .Where(chr => chr.Id != attacker.Id).ToList();
                    var opponent = opponents[new Random().Next(opponents.Count)];
                    int damage = 0;
                    string attackUsed = String.Empty;

                    var attackTypes = Enum.GetValues(typeof(AttackType));
                    AttackType attackType = (AttackType)attackTypes
                        .GetValue(new Random().Next(attackTypes.Length));
                    switch (attackType)
                    {
                        case AttackType.Default:
                        {
                            attackUsed = attacker.Weapon?.Name ?? attacker.Name;
                            damage = InitDefaultAttack(attacker, opponent);
                            break;
                        }
                        case AttackType.Skill:
                        {
                            var skill = attacker.Skills.Count > 0
                                ? attacker.Skills[new Random().Next(attacker.Skills.Count)]
                                : null;
                            Console.WriteLine("skill: {0}", skill);
                            attackUsed = skill?.Name ?? attacker.Name;
                            damage = InitSkillAttack(skill, attacker, opponent);
                            break;
                        }
                    }
                    serviceResponse.Data.Log.Add(
                        $"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {Math.Max(0, damage)} damage"
                        );
                    if (opponent.Hitpoints <= 0)
                    {
                        defeated = true;
                        attacker.Victories++;
                        opponent.Defeats++;
                        serviceResponse.Data.Log.Add($"{opponent.Name} has been defeated!");
                        serviceResponse.Data.Log
                            .Add($"{attacker.Name} wins with {attacker.Hitpoints} HP left!");
                        break;
                    }
                }
            }
            
            characters.ForEach(chr =>
            {
                chr.Fights++;
                chr.Hitpoints = initialHp[chr.Name];
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.ToString();
        }

        return serviceResponse;
    }
}