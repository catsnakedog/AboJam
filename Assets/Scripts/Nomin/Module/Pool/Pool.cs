using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;

/// <summary>
/// <br>풀링할 프리팹을 하이라키에서 연결합니다.</br>
/// <br>프리팹 이름은 중복될 수 없습니다.</br>
/// <br>프리팹은 반드시 IPoolee 를 구현해야 합니다.</br>
/// </summary>
public class Pool : MonoBehaviour
{
    /* Dependency */
    public GameObject[] obj; // 풀링 대상 오브젝트
    public GameObject root; // 모든 풀링 오브젝트의 부모

    /* Field & Property */
    public static Pool instance;
    public Dictionary<string, Queue<GameObject>> pools = new(); // 오브젝트 풀의 집합

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        for (int i = 0; i < obj.Length; i++) pools.Add(obj[i].name, new Queue<GameObject>());
    }

    /* Public Method */
    /// <summary>
    /// 풀에서 오브젝트를 반환합니다.
    /// </summary>
    public GameObject Get(string name)
    {
        if (pools.TryGetValue(name, out var queue))
        {
            GameObject obj = Search(queue) ?? Create(name);
            obj.SetActive(true);
            Load(obj);
            return obj;
        }
        else { Debug.Log($"Pool 에 {name} 프리팹이 할당되지 않았습니다."); return null; }
    }
    /// <summary>
    /// 지정한 이름의 오브젝트의 풀을 초기화 합니다.
    /// </summary>
    public void Clean(string name)
    {
        try { pools[name].Clear(); }
        catch { Debug.Log($"Pool 에 {name} 프리팹이 존재하지 않습니다."); }
    }
    /// <summary>
    /// 풀에 오브젝트를 반환합니다.
    /// </summary>
    public void Return(GameObject obj)
    {
        Save(obj);
        obj.SetActive(false);
        try { pools[obj.name].Enqueue(obj); }
        catch { Debug.Log($"Pool 에 {name} 프리팹이 존재하지 않습니다."); }
    }

    /* Private Method */
    /// <summary>
    /// <br>Pool 에서 오브젝트를 반환합니다.</br>
    /// </summary>
    /// <returns>
    /// <br>Pool 에서 사용 가능한 오브젝트 입니다.</br>
    /// <br>사용 가능한 오브젝트가 없으면 null 을 반환합니다.</br>
    /// </returns>
    private GameObject Search(Queue<GameObject> queue)
    {
        if (queue.Count > 0) return queue.Dequeue();
        else return null;
    }
    /// <summary>
    /// 오브젝트를 새로 생성합니다.
    /// </summary>
    /// <returns>새로 생성된 오브젝트 입니다.</returns>
    private GameObject Create(string name)
    {
        GameObject prefab = Array.Find(this.obj, o => o.name == name); // 이름으로 프리팹 검색
        GameObject obj = Instantiate(prefab, root.transform); // 인스턴스화
        obj.name = name; // 이름 설정 (안하면 Clone 붙어서 Return 시 방해됨)
        return obj;
    }
    /// <summary>
    /// 풀에서 꺼낸 오브젝트의 초기화 메서드 Load 를 실행합니다.
    /// </summary>
    private void Load(GameObject obj)
    {
        foreach (var component in obj.GetComponents<MonoBehaviour>()) if (component is IPoolee ipoolee) { ipoolee.Load(); return; }
    }
    /// <summary>
    /// 풀에서 꺼낸 오브젝트의 데이터 저장 메서드 Save 를 실행합니다.
    /// </summary>
    private void Save(GameObject obj)
    {
        foreach (var component in obj.GetComponents<MonoBehaviour>()) if (component is IPoolee ipoolee) { ipoolee.Save(); return; }
    }
}