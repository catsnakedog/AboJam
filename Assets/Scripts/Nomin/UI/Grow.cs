using System;
using UnityEngine;

public class Grow : MonoBehaviour
{
    /* Dependency */
    private Promotion promotion => Promotion.instance;
    private Reinforcement reinforcement => Reinforcement.instance;
    private Demolition demolition => Demolition.instance;
    private Pool pool => Pool.instance;
    private Tile currentTile => Tile.currentTile;
    private Abocado currentAbocado => Abocado.currentAbocado;
    private Message message => Message.instance;

    /* Field & Property */
    public static Grow instance; // 싱글턴
    public int price;
    public event Action<int> eventGrow;
    public static Action eventPromotion;

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
        demolition.Off();
        promotion.Off();
        gameObject.SetActive(true);
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }

    /* Public Method */
    /// <summary>
    /// 최근 타일의 아보카도에게 먹이를 줍니다.
    /// </summary>
    public void Feed()
    {
        if (currentAbocado.hp.HP_current <= 0) { message.On("그 친구는 무지개다리를 건넜어요..", 2f); Off(); return; }
        if (!currentAbocado.gameObject.activeSelf) { message.On("이미 성장했어요 !", 2f); Off(); return; }
        if (currentAbocado.Level > EnumData.Abocado.Seed) { message.On("이미 성장했어요 !", 2f); Off(); return; }
        if (StaticData.Garu < price) { message.On("보약이 부족해요.", 2f); Off(); return; }

        StaticData.Garu -= price;
        eventGrow?.Invoke(price);
        eventPromotion.Invoke();
        currentAbocado.GrowUp();
        message.On("아보카도의 기분이 좋아졌어요 !", 2f);
        Off();
    }
}
