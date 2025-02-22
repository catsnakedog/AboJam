public class ClashTags : RecordInstance<Table_ClashTags, Record_ClashTags>
{
    /* Intializer & Finalizer & Updater */
    public void Awake()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
    }
}