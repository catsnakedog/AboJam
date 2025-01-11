using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    /* Field & Property */
    public static List<Pooling> instances = new List<Pooling>();
    public List<GameObject> pool { get; private set; } = new List<GameObject>(); // 발사체 풀링
    public GameObject pool_root { get; private set; }
    [HideInInspector] public GameObject obj;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        pool_root = GameObject.Find("@Pooling") ?? new GameObject("@Pooling");
    }
    private void Start()
    {
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
        foreach (var obj in pool) Destroy(obj); // 오브젝트 풀 제거
    }

    /* Public Method */
    /// <summary>
    /// 관리되는 오브젝트를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public GameObject Get()
    {
        return Search() ?? Create();
    }
    /// <summary>
    /// 풀링 오브젝트를 설정합니다.
    /// </summary>
    /// <param name="obj"></param>
    public void Set(GameObject obj)
    {
        Clean();
        this.obj = obj;
    }

    /* Private Method */
    /// <summary>
    /// <br>pool 에서 비활성화된 발사체를 찾습니다.</br>
    /// </summary>
    /// <returns>
    /// <br>pool 에서 사용 가능한 발사체 입니다.</br>
    /// <br>사용 가능한 발사체가 없으면 null 을 반환합니다.</br>
    /// </returns>
    private GameObject Search()
    {
        GameObject go = null;

        // pool 에서 비활성화된 오브젝트를 찾습니다.
        foreach (var obj in pool)
        {
            if (obj.activeSelf == false)
            {
                go = obj;
                go.SetActive(true);
                break;
            }
        }

        return go;
    }
    /// <summary>
    /// 오브젝트를 새로 생성합니다.
    /// </summary>
    /// <returns>새로 생성된 오브젝트 입니다.</returns>
    private GameObject Create()
    {
        GameObject obj = Instantiate(this.obj, pool_root.transform); ;

        pool.Add(obj);
        return obj;
    }
    /// <summary>
    /// 풀을 초기화 합니다.
    /// </summary>
    private void Clean()
    {
        foreach (var obj in pool) Destroy(obj);
        pool.Clear();
    }
}
