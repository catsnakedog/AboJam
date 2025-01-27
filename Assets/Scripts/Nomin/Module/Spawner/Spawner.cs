using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Pool pool => Pool.instance;
    public GameObject[] obj;

    /* Field & Property */
    public static Spawner instance;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;

        // 테스트 코드입니다. 밤마다 스폰시킵니다.
        Date.instance.nightStart.AddListener(() => StartCoroutine(CorSpawn(new Sector("test", 0, 30, 5, 10), obj, 1, 3)));
    }

    /* Public Method */
    /// <summary>
    /// 지정한 영역에 프리팹을 생성합니다.
    /// </summary>
    /// <param name="sector">두 원과 각도로 정의된 영역</param>
    /// <param name="preafabs">랜덤으로 생성될 프리팹</param>
    /// <param name="interval">몬스터 생성 간격</param>
    /// <param name="count">몬스터 생성 수</param>
    /// <returns></returns>
    public IEnumerator CorSpawn(Sector sector, GameObject[] prefabs, float interval, int count)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(interval);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = pool.Get(prefabs[Random.Range(0, prefabs.Length)].name);
            obj.transform.position = transform.position + GetRandomPoint(sector);

            yield return waitForSeconds;
        }
    }

    /* Private Method */
    /// <summary>
    /// 섹터 내에서 랜덤한 위치를 반환합니다.
    /// </summary>
    /// <param name="sector"></param>
    /// <returns></returns>
    private Vector3 GetRandomPoint(Sector sector)
    {
        float radian = Random.Range(sector.angleStart, sector.angleEnd) * Mathf.Deg2Rad;
        float radius = Random.Range(sector.radiusIn, sector.radiusOut);

        return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * radius;
    }
}
