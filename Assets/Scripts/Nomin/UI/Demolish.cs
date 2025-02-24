using UnityEngine;

public class Demolition : MonoBehaviour
{
    /* Dependency */
    private Promotion promotion => Promotion.instance;
    private Reinforcement reinforcement => Reinforcement.instance;
    private Grow grow => Grow.instance;
    private Pool pool => Pool.instance;
    private Tile currentTile => Tile.currentTile;

    /* Field & Property */
    public static Demolition instance; // 싱글턴

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void On()
    {
        reinforcement.Off();
        grow.Off();
        promotion.Off();
        gameObject.SetActive(true);
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }

    /* Public Method */
    /// <summary>
    /// 최근 타일의 오브젝트를 풀로 반환합니다.
    /// </summary>
    public void Demolish()
    {
        pool.Return(currentTile.Go);
        Off();
    }
}
