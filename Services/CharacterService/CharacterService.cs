using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            response.Data = await _context.Characters
                .Select(chr => _mapper.Map<GetCharacterDto>(chr))
                .ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            response.Data = dbCharacters.Select(chr => _mapper.Map<GetCharacterDto>(chr)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try {
                var dbCharacter = await _context.Characters.FirstAsync(chr => chr.Id == id);
                response.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            } catch(Exception e) {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try {
                Character character = await _context.Characters
                    .FirstAsync(chr => chr.Id == updatedCharacter.Id);
                character.Name = updatedCharacter.Name;
                character.Hitpoints = updatedCharacter.Hitpoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);
            } catch(Exception e) {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            try {
                Character character = await _context.Characters.FirstAsync(chr => chr.Id == id);
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                response.Data = await _context.Characters.Select(chr => _mapper.Map<GetCharacterDto>(chr)).ToListAsync();
            } catch(Exception e) {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}