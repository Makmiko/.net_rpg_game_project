using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
    {
        return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetOne(int id)
    {
        var result = await _characterService.GetCharacterById(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter)
    {
        return Created("something + [controller]", await _characterService.AddCharacter(newCharacter));
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateOne(UpdateCharacterDto updatedCharacter)
    {
        var result = await _characterService.UpdateCharacter(updatedCharacter);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> DeleteOne(int id)
    {
        var result = await _characterService.DeleteCharacter(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}