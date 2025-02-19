public class ReinforceCost : RecordInstance<Table_ReinforceCost, Record_ReinforceCost>
{
    /* Intializer & Finalizer & Updater */
    public void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
    }
}