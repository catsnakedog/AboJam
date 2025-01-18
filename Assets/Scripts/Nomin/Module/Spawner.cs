using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /* Dependency */
    /// <summary>
    /// <br>두 원과 각도로 정의된 영역입니다.</br>
    /// <br>HDD 의 Sector 와 매우 유사합니다.</br>
    /// </summary>
    public struct Sector
    {
        float angleStart;
        float angleEnd;
        float radiusIn;
        float radiusOut;
    }
    private Pool pool => Pool.instance;

    /* Field & Property */
    public static Spawner instance;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
    }

    /* Public Method */
    /// <summary>
    /// 지정한 영역에 프리팹을 생성합니다.
    /// </summary>
    /// <param name="sector">두 원과 각도로 정의된 영역</param>
    /// <param name="preafabs">랜덤으로 생성될 프리팹</param>
    /// <param name="delay">몬스터 생성 간격</param>
    /// <param name="count">몬스터 생성 수</param>
    /// <returns></returns>
    public IEnumerator CorSpawn(Sector sector, GameObject[] prefabs, float delay, int count)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(delay);

        for (int i = 0; i < count; i++)
        {
            

            yield return waitForSeconds;
        }
    }

    /* Private Method */

}
