using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public interface IEnemy
{
    public static List<IEnemy> instances = new();
    public static IEnemy currentEnemy;
}