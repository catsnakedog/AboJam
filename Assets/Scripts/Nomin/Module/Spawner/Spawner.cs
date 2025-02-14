using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    /* Dependency */
    private Pool pool => Pool.instance;
    private Database_AboJam database_abojam => Database_AboJam.instance;
    private Message message => Message.instance;
    private Date date => Date.instance;

    /* Field & Property */
    public static Spawner instance;
    public int waveIndex = 1;
    public bool waveEnd { get; set; } = false;
    private int spriteOrderIndex = 0;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        date.nightStart.AddListener(() => StartCoroutine(CorWave()));
    }

    /* Public Method */
    /// <summary>
    /// 몬스터 웨이브가 시작됩니다.
    /// </summary>
    public IEnumerator CorWave()
    {
        if (waveIndex >= database_abojam.Wave.Count) { message.On("다음 웨이브가 없습니다.", 2f); yield break; }

        // 런타임 테이블에서 다음에 진행할 웨이브를 가져옵니다.
        Wave wave = new Wave();
        database_abojam.ExportWave(database_abojam.Wave[waveIndex].ID, ref wave.delay, ref wave.spawn);
        waveIndex++;

        // 웨이브의 모든 스폰 순차 생성
        for (int i = 0; i < wave.delay.Length; i++)
        {
            if (i == wave.delay.Length - 1) { message.On("곧 아침이 옵니다. 조금만 더 버티세요 !", 2f); }
            yield return new WaitForSeconds(wave.delay[i]);

            // 각 스폰 마다 몬스터 생성
            for (int j = 0; j < wave.spawn[i].count; j++)
            {
                WaitForSeconds waitForSeconds = new(wave.spawn[i].interval);

                // 생성
                int random = Random.Range(0, wave.spawn[i].spawnee.prefabs.Length);
                GameObject obj = pool.Get(wave.spawn[i].spawnee.prefabs[random].name);
                obj.transform.position = transform.position + GetRandomPoint(wave.spawn[i].sector);
                SetSpriteOrder(obj.GetComponent<SpriteRenderer>());

                // 지정된 레벨까지 업그레이드
                IEnemy enemy = obj.GetComponent<IEnemy>();
                enemy.Level = wave.spawn[i].spawnee.levels[random];
                for (int k = 0; k < enemy.Level; k++) enemy.Reinforce();

                yield return waitForSeconds;
            }
        }

        waveEnd = true;
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
    /// <summary>
    /// 스프라이트의 순서를 생성 순으로 정렬합니다.
    /// </summary>
    private void SetSpriteOrder(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.sortingOrder = spriteOrderIndex;
        spriteOrderIndex++;
        if (spriteOrderIndex == 32768) spriteOrderIndex = 0;
    }
}
