using UnityEngine;
using System;
using TMPro;
using Unity.VisualScripting;

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
    public static Action eventDemolish;
    public static Action eventPopUp;
    public static Action eventPopDown;
    public TextMeshProUGUI tmp;
    public bool Active { get; set; } = true;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void On()
    {
        if (Active == false) return;
        reinforcement.Off();
        grow.Off();
        promotion.Off();
        gameObject.SetActive(true);
        eventPopUp.Invoke();
        string cultivateText = "정말 철거하시겠습니까?\n\n아무것도 돌려받을 수 없습니다.";
        string defaultText = "정말 철거하시겠습니까?\n\n(30 가루 환급)";
        SetText(cultivateText, defaultText);

        void SetText(string cultivateText, string defaultText)
        {
            switch (isCultivate())
            {
                case true:
                    tmp.text = cultivateText;
                    break;
                default:
                    tmp.text = defaultText;
                    break;
            }

            bool isCultivate()
            {
                Abocado abocado = currentTile.Go.GetComponent<Abocado>();
                if (abocado != null)
                    if (abocado.Level == EnumData.Abocado.Cultivated) return true;

                return false;
            }
        }
    }
    public void Off()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            eventPopDown.Invoke();
        }
    }

    /* Public Method */
    /// <summary>
    /// 최근 타일의 오브젝트를 풀로 반환합니다.
    /// </summary>
    public void Demolish()
    {
        eventDemolish.Invoke();
        Refund();
        pool.Return(currentTile.Go);
        Off();

        // 경작지를 제외하고 30 가루를 환급합니다.
        void Refund()
        {
            // 경작지는 환급 X
            Abocado abocado = currentTile.Go.GetComponent<Abocado>();
            if (abocado != null)
                if (abocado.Level == EnumData.Abocado.Cultivated) return;

            StaticData.Garu += 30;
        }
    }
}
