namespace dotnet_rpg.Models;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = "Darth Vader";
    public int Hitpoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public RpgClass Class { get; set; } = RpgClass.Knight;
    public User? User { get; set; }
    public Weapon? Weapon { get; set; }
    public List<Skill> Skills { get; set; } = new List<Skill>();
    public int Fights { get; set; }
    public int Victories { get; set; }
    public int Defeats { get; set; }
}