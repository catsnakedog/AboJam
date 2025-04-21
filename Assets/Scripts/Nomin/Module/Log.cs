using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Collections;
using OfficeOpenXml;
using System.ComponentModel;

[System.Serializable]
public struct DayLog
{
    public int day;
    // AddTowerLog
    public int guard;
    public int auto;
    public int splash;
    public int heal;
    public int production;
    // AddCharacterLog
    public int character;
    // AddCultivatedLog
    public int cultivated;
    // AddWeaponLog
    public int weapon;
    // AddGrowLog
    public int grow;
    public int treeCount;
    public int productionCount;
    public int remainAbocado;
    public int remainGaru;
}

internal class Log : MonoBehaviour
{
    private List<DayLog> dayLogs = new List<DayLog>();
    private DayLog currentLog = new DayLog();
    public static Action<int> addWeaponLog;
    public static Action<int> addCharacterLog;

    private void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(0.5f);
        Date.instance.morningStart.AddListener(AddDayLog);
        Promotion.instance.eventPromote += AddTowerLog;
        Reinforcement.instance.eventReinforce += AddTowerLog;
        Farming.instance.eventCultivated += AddCultivatedLog;
        Grow.instance.eventGrow += AddGrowLog;
        addWeaponLog += AddWeaponLog;
        addCharacterLog += AddCharacterLog;
    }

    /* Public Method */
    /// <summary>
    /// Application.persistentDataPath 에 저장합니다.
    /// </summary>
    /// <param name="filename">파일 이름</param>
    public void Save(string filename = "DayLog.xlsx")
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        FileInfo fileInfo = new FileInfo(path);

        // 파일이 없으면 새로 생성
        if (!fileInfo.Exists)
        {
            using (var package = new ExcelPackage(fileInfo))
            {
                // 첫 시트 생성
                var ws = package.Workbook.Worksheets.Add("GameDayData_1");
                WriteSheet(ws);
                package.Save();
            }
            Debug.Log($"엑셀 새로 저장됨: {path}");
            return;
        }

        // 파일이 있으면 기존 파일 열고 시트 추가
        using (var package = new ExcelPackage(fileInfo))
        {
            int sheetIndex = 1;
            string sheetName;
            do
            {
                sheetName = $"GameDayData_{sheetIndex}";
                sheetIndex++;
            } while (package.Workbook.Worksheets[sheetName] != null);

            var ws = package.Workbook.Worksheets.Add(sheetName);
            WriteSheet(ws);
            package.Save();
            Debug.Log($"엑셀 시트 추가됨: {path} - 시트명: {sheetName}");
        }

        void WriteSheet(ExcelWorksheet ws)
        {
            string[] headers = new string[]
            {
            "Day", "Guard", "Auto", "Splash", "Heal", "Character", "Cultivated",
            "Weapon", "Production", "Grow", "TreeCount", "ProductionCount",
            "RemainAbocado", "RemainGaru"
            };

            for (int i = 0; i < headers.Length; i++)
                ws.Cells[1, i + 1].Value = headers[i];

            for (int i = 0; i < dayLogs.Count; i++)
            {
                var d = dayLogs[i];
                ws.Cells[i + 2, 1].Value = d.day;
                ws.Cells[i + 2, 2].Value = d.guard;
                ws.Cells[i + 2, 3].Value = d.auto;
                ws.Cells[i + 2, 4].Value = d.splash;
                ws.Cells[i + 2, 5].Value = d.heal;
                ws.Cells[i + 2, 6].Value = d.character;
                ws.Cells[i + 2, 7].Value = d.cultivated;
                ws.Cells[i + 2, 8].Value = d.weapon;
                ws.Cells[i + 2, 9].Value = d.production;
                ws.Cells[i + 2, 10].Value = d.grow;
                ws.Cells[i + 2, 11].Value = d.treeCount;
                ws.Cells[i + 2, 12].Value = d.productionCount;
                ws.Cells[i + 2, 13].Value = d.remainAbocado;
                ws.Cells[i + 2, 14].Value = d.remainGaru;
            }
        }
    }

    /* Private Method */
    /// <summary>
    /// <br>DayLog 를 추가합니다. (Date.morningStart 바운드)</br>
    /// </summary>
    private void AddDayLog()
    {
        currentLog.day = Date.instance.dateTime.Day;
        currentLog.treeCount = GetTreeCount();
        currentLog.productionCount = GetProductionCount();
        currentLog.remainAbocado = StaticData.Abocado;
        currentLog.remainGaru = StaticData.Garu;

        dayLogs.Add(currentLog);
        currentLog = new DayLog();

        int GetTreeCount()
        {
            int count = 0;

            foreach (Abocado abocado in Abocado.instances)
                if (abocado.gameObject.activeSelf && abocado.Level >= EnumData.Abocado.Tree && abocado.Quality == 0)
                    count++;

            return count;
        }
        int GetProductionCount()
        {
            int count = 0;

            foreach (Abocado abocado in Abocado.instances)
                if (abocado.gameObject.activeSelf && abocado.Quality > 0)
                    count++;

            return count;
        }
    }
    /// <summary>
    /// 타워 건설 / 업그레이드 로그를 기록합니다.
    /// </summary>
    private void AddTowerLog(EnumData.TowerType towerType, int price)
    {
        switch (towerType)
        {
            case EnumData.TowerType.Guard:
                currentLog.guard += price;
                break;
            case EnumData.TowerType.Auto:
                currentLog.auto += price;
                break;
            case EnumData.TowerType.Splash:
                currentLog.splash += price;
                break;
            case EnumData.TowerType.Production:
                currentLog.production += price;
                break;
            case EnumData.TowerType.Heal:
                currentLog.heal += price;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 캐릭터 업글레이드 로그를 기록합니다.
    /// </summary>
    private void AddCharacterLog(int price)
    {
        currentLog.character += price;
    }
    /// <summary>
    /// 경작 로그를 기록합니다.
    /// </summary>
    private void AddCultivatedLog(int price)
    {
        currentLog.cultivated += price;
    }
    /// <summary>
    /// 무기 구매 로그를 기록합니다.
    /// </summary>
    private void AddWeaponLog(int price)
    {
        currentLog.weapon += price;
    }
    /// <summary>
    /// 강제 성장 로그를 기록합니다.
    /// </summary>
    private void AddGrowLog(int price)
    {
        currentLog.grow += price;
    }
}
