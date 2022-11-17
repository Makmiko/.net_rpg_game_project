using System.Security.Claims;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.WeaponService;

public class WeaponService : IWeaponService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();
        try
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier));
            var character = await _context.Characters
                .Include(chr => chr.Weapon)
                .FirstOrDefaultAsync(
                    chr => chr.Id == newWeapon.CharacterId && chr.User.Id == userId
                );

            if (character == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character not found";
                return serviceResponse;
            }
            else if (character.Weapon != null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This character already has a weapon";
                return serviceResponse;
            }

            Weapon weapon = new Weapon()
            {
                Name = newWeapon.Name,
                Damage = newWeapon.Damage,
                Character = character,
            };
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }
}