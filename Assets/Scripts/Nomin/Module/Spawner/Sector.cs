public class Sector : RecordInstance<Table_Sector, Record_Sector>
{
    public float angleStart;
    public float angleEnd;
    public float radiusIn;
    public float radiusOut;

    /* Intializer & Finalizer & Updater */
    public void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
    }
}