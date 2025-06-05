using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class OnOff : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /* Dependency */
    public static OnOff instance;
    public Button button;
    public AnimationClick animationClick;
    public Sprite on;
    public Sprite off;
    public GameObject panel;
    private Image img;
    public static Action eventClick;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        img = GetComponent<Image>();
        button.onClick.AddListener(() => Switch());
    }

    /* Public Method */
    public void Switch()
    {
        eventClick.Invoke();
        animationClick.OnClick();
        panel.SetActive(!panel.activeSelf);
    }

    /* Private Method */
    public void OnPointerEnter(PointerEventData eventData)
    {
        img.sprite = on;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        img.sprite = off;
    }
}
