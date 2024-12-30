using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite on;
    public Sprite off;
    public GameObject shopPannel;
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.sprite = on;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.sprite = off;
    }

    public void ShopPannelOnOff()
    {
        shopPannel.SetActive(!shopPannel.activeSelf);
    }
}
