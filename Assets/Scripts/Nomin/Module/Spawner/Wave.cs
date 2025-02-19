public class Wave : RecordInstance<Table_Wave, Record_Wave>
{
    public float[] delay;
    public Spawn[] spawn;

    /* Intializer & Finalizer & Updater */
    public void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
    }
}