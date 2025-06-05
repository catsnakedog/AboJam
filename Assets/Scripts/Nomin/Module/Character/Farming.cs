using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

public class Farming : MonoBehaviour
{
    /* Dependency */
    [SerializeField] private Player player;
    [SerializeField] private GameObject gauge;
    private Grid grid => Grid.instance;
    private Pool pool => Pool.instance;
    private Message message => Message.instance;

    /* Field & Property */
    public static Farming instance;
    public float cultivateTime = 1f; // 경작 시간
    public int price = 5; // 경작 비용
    public float waitForCultivateGaugeRatio; // 이전 경작이 일정 비율 이상 진행 중일 시 대기
    public event Action<int> eventCultivated;
    private Coroutine corCultivate;
    private Coroutine corWaitCultivate;
    private Coroutine corMove;
    private Coroutine corGauge;
    private GameObject gaugeObj;
    public static Action<bool> eventMove;

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        StopAllCoroutines();
    }

    /* Public Method */
    /// <summary>
    /// 타일 위치까지 이동한 후 땅을 경작합니다.
    /// </summary>
    public void Cultivate(Tile tile)
    {
        // 이미 타일에 설치된 오브젝트가 있으면 리턴합니다.
        if (tile.Go != null) return;

        // 이전 경작이 일정 비율 이상 진행 중이면 기다린 후 경작 시작
        if (corGauge != null)
            if (gaugeObj.GetComponent<Gauge>().ratio >= waitForCultivateGaugeRatio)
            {
                if (corWaitCultivate != null) StopCoroutine(CorWaitCultivate(tile));
                corWaitCultivate = StartCoroutine(CorWaitCultivate(tile));
                return;
            }

        // 즉시 경작 시작
        if (corCultivate != null) StopCultivate();
        corCultivate = StartCoroutine(CorCultivate(tile));
    }
    /// <summary>
    /// 이전 경작이 종료된 이후에 다음 경작을 시도합니다.
    /// </summary>
    public IEnumerator CorWaitCultivate(Tile tile)
    {
        while (true)
        {
            if (corGauge == null)
            {
                if (corCultivate != null) StopCultivate();
                corCultivate = StartCoroutine(CorCultivate(tile));
                corWaitCultivate = null;
                yield break;
            }
            else yield return new WaitForFixedUpdate();
        }
    }
    /// <summary>
    /// 즉시 경작을 시작합니다.
    /// </summary>
    public IEnumerator CorCultivate(Tile tile)
    {
        yield return corMove = StartCoroutine(CorMove(tile.pos, grid.CellWidth));
        if (StaticData.Garu < price)
        {
            message.On($"개간하려면 보약 {price} 개를 먹고 힘을 내야 해요 !", 2f);
            StopCultivate();
            yield break;
        }
        yield return corGauge = StartCoroutine(CorGauge(cultivateTime, 100f, 0.016f));
        if (StaticData.Garu < price)
        {
            message.On($"개간하려면 보약 {price} 개를 먹고 힘을 내야 해요 !", 2f);
            StopCultivate();
            yield break;
        }
        else StaticData.Garu -= price;
        eventCultivated?.Invoke(price);

        if (tile.Go == null) tile.Bind(pool.Get("Abocado"), EnumData.TileIndex.AboCado);
        else { UnityEngine.Debug.Log($"타일 ({tile.i}, {tile.j}) 에 이미 {tile.Go.name} 가 바인딩 되어 있습니다."); };
    }
    /// <summary>
    /// 경작을 중단합니다.
    /// </summary>
    public void StopCultivate()
    {
        if (gaugeObj != null) { Destroy(gaugeObj); gaugeObj = null; }
        if (corGauge != null) { StopCoroutine(corGauge); corGauge = null; }
        if (corMove != null) { StopCoroutine(corMove); player.PlayerMovement._movement = Vector2.zero; corMove = null; }
        if (corCultivate != null) { StopCoroutine(corCultivate); corCultivate = null; }
    }

    /* Private Method */
    /// <summary>
    /// 캐릭터를 특정 위치로 일정 거리 이내까지 이동시킵니다.
    /// </summary>
    private IEnumerator CorMove(Vector3 worldPos, float distance)
    {
        eventMove.Invoke(true);
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.016f);

        while (Vector2.Distance(transform.position, worldPos) > distance)
        {
            float distance_x = worldPos.x - player.transform.position.x;
            float distance_y = worldPos.y - player.transform.position.y;

            // 캐릭터가 오른쪽이면 왼쪽으로 이동
            if (distance_x < -(distance * 0.1f)) player.PlayerMovement._movement.x = -1;
            // 캐릭터가 왼쪽이면 오른쪽으로 이동
            else if (distance_x > (distance * 0.1f)) player.PlayerMovement._movement.x = 1;
            // 캐릭터가 distance * 0.1 범위 이내면 좌우 이동 중단
            else player.PlayerMovement._movement.x = 0;

            // 캐릭터가 위면 아래로 이동
            if (distance_y < -(distance * 0.1f)) player.PlayerMovement._movement.y = -1;
            // 캐릭터가 아래면 왼쪽으로 이동
            else if (distance_y > (distance * 0.1f)) player.PlayerMovement._movement.y = 1;
            // 캐릭터가 distance * 0.1 범위 이내면 상하 이동 중단
            else player.PlayerMovement._movement.y = 0;

            yield return waitForSeconds;
        }

        eventMove.Invoke(false);
        player.PlayerMovement._movement = Vector2.zero;
    }
    /// <summary>
    /// seconds 초 동안 게이지를 percent % 채웁니다.
    /// </summary>
    /// <param name="delay">회복 틱 사이 간격</param>
    private IEnumerator CorGauge(float seconds, float percent, float delay)
    {
        Vector3 pos = transform.position + new Vector3(0, 1, 0);
        gaugeObj = Instantiate(this.gauge, pos, Quaternion.identity);
        Gauge gauge = gaugeObj.GetComponent<Gauge>();

        float healPerTick = (percent / seconds) / (seconds / delay) * 0.01f;

        while (gauge.current < gauge.max)
        {
            gauge.Fill(gauge.max * healPerTick);
            yield return new WaitForSeconds(delay);
        }

        if(gaugeObj != null) Destroy(gaugeObj);
        corGauge = null;
    }
}
