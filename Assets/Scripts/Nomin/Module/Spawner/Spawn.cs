public class Spawn : RecordInstance<Table_Spawn, Record_Spawn>
{
    public Sector sector;
    public Spawnee spawnee;
    public float interval;
    public int count;

    /* Intializer & Finalizer & Updater */
    public void Awake()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
    }
}