using UnityEngine;

public class Spawnee : RecordInstance<Table_Spawnee, Record_Spawnee>
{
    public GameObject[] prefabs;
    public int[] levels;

    /* Intializer & Finalizer & Updater */
    public void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
    }
}