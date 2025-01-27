using System.Collections.Generic;

public class Wave
{
    public static Wave currentWave;
    public static List<Wave> instances = new List<Wave>();

    public string ID;
    public List<Dictionary<Spawn, float>> spawnList = new();
}

