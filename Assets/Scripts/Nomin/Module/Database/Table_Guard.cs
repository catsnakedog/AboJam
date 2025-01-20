using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 PK 입니다.</br>
/// <br>CONSTRAINT : 컬럼 이름이 서버 테이블과 정확히 일치해야 합니다.</br>
/// </summary>
public class Table_Guard
{
    /* Field & Property */
    public string guardID;
    public float hpMultiply;

    /* Intializer & Finalizer & Updater */
    public Table_Guard(string guardID, float hpMultiply)
    {
        this.guardID = guardID;
        this.hpMultiply = hpMultiply;
    }
}