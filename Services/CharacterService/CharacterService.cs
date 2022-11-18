using System.Security.Claims;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    private int GetUserId()
    {
        return int.Parse(_httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();
        var dbCharacters = await _context.Characters
            .Include(chr => chr.Weapon)
            .Include(chr => chr.Skills)
            .Where(chr => chr.User.Id == GetUserId())
            .ToListAsync();
        response.Data = dbCharacters.Select(chr => _mapper.Map<GetCharacterDto>(chr)).ToList();
        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        var response = new ServiceResponse<GetCharacterDto>();
        try
        {
            var dbCharacter = await _context.Characters
                .Include(chr => chr.Weapon)
                .Include(chr => chr.Skills)
                .FirstAsync(
                    chr => chr.Id == id && chr.User.Id == GetUserId()
                );
            response.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();
        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _context.Users.FirstAsync(usr => usr.Id == GetUserId());
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();
        response.Data = await _context.Characters
            .Include(chr => chr.Weapon)
            .Include(chr => chr.Skills)
            .Where(chr => chr.User.Id == GetUserId())
            .Select(chr => _mapper.Map<GetCharacterDto>(chr))
            .ToListAsync();
        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        var response = new ServiceResponse<GetCharacterDto>();
        try
        {
            var character = await _context.Characters
                .Include(chr => chr.User)
                .Include(chr => chr.Weapon)
                .Include(chr => chr.Skills)
                .FirstAsync(chr => chr.Id == updatedCharacter.Id);

            if (character.User.Id == GetUserId())
            {
                character.Name = updatedCharacter.Name;
                character.Hitpoints = updatedCharacter.Hitpoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            else
            {
                response.Success = false;
                response.Message = "Character not found";
            }
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();
        try
        {
            var character = await _context.Characters
                .FirstAsync(chr => chr.Id == id && chr.User.Id == GetUserId());
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            response.Data = await _context.Characters
                .Include(chr => chr.Weapon)
                .Include(chr => chr.Skills)
                .Where(chr => chr.User.Id == GetUserId())
                .Select(chr => _mapper.Map<GetCharacterDto>(chr))
                .ToListAsync();
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
        var response = new ServiceResponse<GetCharacterDto>();
        try
        {
            var character = await _context.Characters
                .Include(chr => chr.Weapon)
                .Include(chr => chr.Skills)
                .FirstOrDefaultAsync(chr => chr.Id == newCharacterSkill.CharacterId
                                            && chr.User.Id == GetUserId());
            if (character == null)
            {
                response.Success = false;
                response.Message = "Character not found";
                return response;
            }

            var skill = await _context.Skills
                .FirstOrDefaultAsync(skill => skill.Id == newCharacterSkill.SkillId);
            if (skill == null)
            {
                response.Success = false;
                response.Message = "Skill not found";
                return response;
            }

            character.Skills.Add(skill);

            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}