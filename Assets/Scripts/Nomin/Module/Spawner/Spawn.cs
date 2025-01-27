using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Timeline.Actions;
using UnityEngine.EventSystems;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public struct Spawn
{
    /* Field & Property */
    public string ID;
    public string sectorID;
    public string spawneeID;
    public float interval;
    public int count;
}