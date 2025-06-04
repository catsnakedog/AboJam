using System.Collections.Generic;

public interface IEnemy
{
    public static List<IEnemy> instances = new();
    public static IEnemy currentEnemy;
    public void Reinforce();
    public void Promotion(EnumData.SpecialLevel level);
    public int Level { get; set; }
}