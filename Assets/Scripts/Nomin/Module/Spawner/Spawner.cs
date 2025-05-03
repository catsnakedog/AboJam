using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
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
    public Coroutine lastCor;
    public Coroutine lastWaveCor;
    public List<Coroutine> lastSpawnCors = new List<Coroutine>();
    private int spriteOrderIndex = 0;
    private int corSpawnEndCount = 0;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        date.nightStart.AddListener(() => { lastCor = StartCoroutine(CorWave()); });
    }

    /* Public Method */
    /// <summary>
    /// 몬스터 웨이브가 시작됩니다.
    /// </summary>
    public IEnumerator CorWave()
    {
        if (waveIndex >= database_abojam.Wave.Count) { message.On("다음 웨이브가 없습니다.", 2f); yield break; }
        waveEnd = false;

        // 런타임 테이블에서 다음에 진행할 웨이브를 가져옵니다.
        Wave wave = new Wave();
        database_abojam.ExportWave(database_abojam.Wave[0].TableName + waveIndex, ref wave.delay, ref wave.spawn);
        waveIndex++;

        // 웨이브의 모든 스폰 순차 생성
        int corSpawnLength = wave.delay.Length;
        corSpawnEndCount = 0;
        lastSpawnCors.Clear();
        for (int i = 0; i < corSpawnLength; i++) lastSpawnCors.Add(StartCoroutine(CorSpawn(wave.spawn[i], wave.delay[i])));

        // 모든 스폰 완료될 때 까지 대기
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
        while (corSpawnEndCount < corSpawnLength) yield return waitForSeconds;
        waveEnd = true;
    }
    /// <summary>
    /// 현재 웨이브를 강제 종료합니다.
    /// </summary>
    public void StopWave()
    {
        if(lastSpawnCors != null) foreach (Coroutine corSpawn in lastSpawnCors) StopCoroutine(corSpawn);
        waveEnd = true;
        if(lastCor != null) StopCoroutine(lastCor);
    }
    /// <summary>
    /// <br>지정한 Sector 내에 프리팹을 스폰시킵니다.</br>
    /// <br>Pool 에 등록된 프리팹만 가능합니다.</br>
    /// </summary>
    public void Spawn(GameObject prefab, Vector3 position)
    {
        GameObject obj = pool.Get(prefab.name);
        obj.transform.position = position;
        SetSpriteOrder(obj.GetComponent<SpriteRenderer>());
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
    /// <summary>
    /// delay 동안 대기 후 Spawn 을 시작합니다.
    /// </summary>
    private IEnumerator CorSpawn(Spawn spawn, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 각 스폰 마다 몬스터 생성
        for (int j = 0; j < spawn.count; j++)
        {
            WaitForSeconds waitForSeconds = new(spawn.interval);

            // 생성
            int random = Random.Range(0, spawn.spawnee.prefabs.Length);
            GameObject obj = pool.Get(spawn.spawnee.prefabs[random].name);
            obj.transform.position = transform.position + GetRandomPoint(spawn.sector);
            SetSpriteOrder(obj.GetComponent<SpriteRenderer>());

            // 지정된 레벨까지 업그레이드
            IEnemy enemy = obj.GetComponent<IEnemy>();
            enemy.Level = spawn.spawnee.levels[random];
            for (int k = 0; k < enemy.Level; k++) enemy.Reinforce();

            yield return waitForSeconds;
        }

        corSpawnEndCount++;
    }
}