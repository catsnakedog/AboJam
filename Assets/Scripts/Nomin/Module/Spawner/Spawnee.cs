using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Timeline.Actions;
using UnityEngine.EventSystems;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Spawnee : RecordInstance<Table_Spawnee, Record_Spawnee>
{
    public GameObject[] prefabs;

    /* Intializer & Finalizer & Updater */
    public void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
    }
}