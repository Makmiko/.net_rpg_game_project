namespace dotnet_rpg.Dtos.Character;

public class AddCharacterDto
{
    public string Name { get; set; } = string.Empty;
    public int Hitpoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public RpgClass Class { get; set; } = RpgClass.Knight;
}