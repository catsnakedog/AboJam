
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// <br>런타임 레코드 인스턴스가 상속받는 클래스</br>
/// <br>반드시 런타임 DB 의 Awake 이후 이 스크립트의 Start 가 실행되어야 합니다.</br>
/// </summary>
/// <typeparam name="T1">테이블 타입</typeparam>
/// <typeparam name="T2">레코드 타입 (ScriptableObject)</typeparam>
public class RecordInstanceBase<T1, T2> : MonoBehaviour where T1 : ITable where T2 : IRecord
{
    /* Dependency */
    private Database_AboJam database_aboJam => Database_AboJam.instance;
    public T2 initialRecord;
    public bool startFlag = false;

    /* Intializer & Finalizer & Updater */
    /// <summary>
    /// 런타임 테이블에 초기값 레코드 SO 를 등록합니다.
    /// </summary>
    public void Start()
    {
        if (initialRecord == null) Debug.Log($"{gameObject.name} 의 Scriptable Object 가 할당되지 않았습니다.");

        // 런타임 DB 에서 테이블을 가져옵니다.
        List<T1> table = GetField<List<T1>>(database_aboJam)[0];

        // 런타임 테이블에 ID 가 같은 레코드가 존재하지 않으면 등록
        if (table.FirstOrDefault(record => record.ID == initialRecord.ID) == null)
            table.Add((T1)Activator.CreateInstance(typeof(T1), GetFields(initialRecord)));
    }

    /// <summary>
    /// 인스턴스에서 특정 타입의 모든 필드를 반환합니다.
    /// </summary>
    /// <param name="instance">인스턴스</param>
    private T[] GetField<T>(System.Object instance)
    {
        FieldInfo[] fieldInfos = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        List<T> targetFields = new();

        foreach (FieldInfo field in fieldInfos) if (field.FieldType == typeof(T)) targetFields.Add((T)field.GetValue(instance));

        return targetFields.ToArray();
    }
    /// <summary>
    /// 인스턴스의 모든 필드를 반환합니다.
    /// </summary>
    /// <param name="instance">인스턴스</param>
    /// <returns></returns>
    private System.Object[] GetFields(System.Object instance)
    {
        FieldInfo[] fieldInfos = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        List<System.Object> targetFields = new();

        foreach (FieldInfo field in fieldInfos) targetFields.Add((System.Object)field.GetValue(instance));

        return targetFields.ToArray();
    }
}