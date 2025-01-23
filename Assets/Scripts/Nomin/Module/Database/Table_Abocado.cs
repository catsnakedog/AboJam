using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 PK 입니다.</br>
/// <br>CONSTRAINT : 컬럼 이름이 서버 테이블과 정확히 일치해야 합니다.</br>
/// </summary>
public class Table_Abocado : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Abocado";
    public string ID { get; set; }
    public EnumData.Abocado level;
    public int quality;
    public int quality_max;
    public int harvest;
    public int harvestPlus;

    /* Intializer & Finalizer & Updater */
    public Table_Abocado(string ID, EnumData.Abocado level, int quality, int quality_max, int harvest, int harvestPlus)
    {
        this.ID = ID;
        this.level = level;
        this.quality = quality;
        this.quality_max = quality_max;
        this.harvest = harvest;
        this.harvestPlus = harvestPlus;
    }
}