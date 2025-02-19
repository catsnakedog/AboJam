using System.Collections.Generic;

public interface IEnemy
{
    public static List<IEnemy> instances = new();
    public static IEnemy currentEnemy;
    public void Reinforce();
    public int Level { get; set; }
}