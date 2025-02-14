using Synty.Interface.FantasyWarriorHUD.Samples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ObjectPool;

public class Skill : MonoBehaviour
{
    /* Dependency */
    [SerializeField] private Player player;
    [SerializeField] private SampleButtonAction sampleButtonAction;
    [SerializeField] private GameObject coolTimer;
    private Pool pool => Pool.instance;
    private Grid grid => Grid.instance;

    /* Field & Initializer */
    public string[] clashTags = { "Enemies" };

    [Header("Meteor")]
    public GameObject meteorite;
    public GameObject explosion;
    public float cooldown;
    public int range;
    public int count;
    public float seconds;
    public float speed;

    /* Initalizer & Finalizer & Updater */
    private void Start()
    {
        sampleButtonAction.activeTime = cooldown;
    }

    /* Public Method */
    /// <summary>
    /// 메테오를 시전합니다.
    /// </summary>
    /// <param name="range">공격 범위 (타일)</param>
    /// <param name="count">운석 개수</param>
    /// <param name="seconds">총 공격 시간</param>
    public void Meteor()
    {
        StartCoroutine(CorMeteor(range, count, seconds, speed));

        coolTimer.SetActive(true);
        coolTimer.GetComponent<CoolTimer>().Go(cooldown);
    }
    private IEnumerator CorMeteor(int range, int count, float seconds, float speed)
    {
        // 플레이어랑 가장 가까운 타일
        Tile playerTile = grid.GetNearestTile(player.transform.position);

        // 범위 내 랜덤한 count 개의 좌표 지정
        List<(int, int)> coords = GetRandomManhattanPoints(playerTile.i, playerTile.j, range, count);

        // 맵 범위를 벗어난 좌표 제거
        coords.RemoveAll(coord => coord.Item1 < 0 || coord.Item1 >= grid.Column || coord.Item2 < 0 || coord.Item2 >= grid.Row);

        // 좌표를 타일로 변환 후 랜덤 순으로 섞기
        List<Tile> tiles = new List<Tile>();
        foreach ((int, int) coord in coords) tiles.Add(grid.GetTile(coord));
        tiles = tiles.OrderBy(x => Random.value).ToList();

        // 각 타일 폭격
        float delay = seconds / count;
        WaitForSeconds waitForSeconds = new WaitForSeconds(delay);
        foreach (Tile tile in tiles)
        {
            StartCoroutine(Airstrike(tile.pos + new Vector2(10, 10), tile.pos, speed));
            yield return waitForSeconds;
        }
    }

    /// <summary>
    /// <br>A 위치에서 B 위치로 폭격을 가합니다.</br>
    /// </summary>
    /// <param name="startPos">낙하 속도</param>
    private IEnumerator Airstrike(Vector2 startPos, Vector2 endPos, float speed)
    {
        // meteorite 방향 계산 및 인스턴스화
        Vector2 direction = (startPos - endPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //GameObject meteorite = Instantiate(this.meteorite, startPos, Quaternion.Euler(0, 0, angle));
        GameObject meteorite = pool.Get(this.meteorite.name);
        meteorite.transform.position = startPos;
        meteorite.transform.rotation = Quaternion.Euler(0, 0, angle);

        WaitForSeconds waitForSeconds = new WaitForSeconds(0.016f);

        while (true)
        {
            meteorite.transform.position = Vector3.MoveTowards(meteorite.transform.position, endPos, speed);

            if (Vector3.Distance(meteorite.transform.position, endPos) < grid.CellWidth * 0.1f)
            {
                // 폭발 생성
                GameObject explosion = pool.Get(this.explosion.name);
                explosion.transform.position = endPos;
                explosion.GetComponent<Explosion>().Explode(clashTags);
                //Destroy(meteorite);
                pool.Return(meteorite);

                break;
            }
                yield return waitForSeconds;
        }
    }
    /// <summary>
    /// (i, j) 기준 맨해튼 거리 range 이내에서 랜덤한 N개의 좌표를 반환합니다.
    /// </summary>
    private List<(int, int)> GetRandomManhattanPoints(int i, int j, int range, int n)
    {
        HashSet<(int, int)> validPoints = new HashSet<(int, int)>(); // 중복 방지

        // 🔹 맨해튼 거리 조건을 만족하는 모든 좌표 찾기
        for (int x = i - range; x <= i + range; x++)
        {
            for (int y = j - range; y <= j + range; y++)
            {
                if (ManhattanDistance(i, j, x, y) <= range)
                {
                    validPoints.Add((x, y));
                }
            }
        }

        // 🔹 좌표 리스트에서 랜덤하게 N개 선택
        List<(int, int)> validList = new List<(int, int)>(validPoints);
        List<(int, int)> result = new List<(int, int)>();

        for (int count = 0; count < n && validList.Count > 0; count++)
        {
            int randomIndex = Random.Range(0, validList.Count);
            result.Add(validList[randomIndex]);
            validList.RemoveAt(randomIndex); // 중복 방지
        }

        return result;
    }
    /// <summary>
    /// 맨해튼 거리 계산
    /// </summary>
    private int ManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }
}

