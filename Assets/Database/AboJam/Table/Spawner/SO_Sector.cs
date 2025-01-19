using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_Sectors", menuName = "Database/SO_Spawner/SO_Sectors", order = int.MaxValue)]
public class Sectors : ScriptableObject
{
    public Sector[] sectors;


}