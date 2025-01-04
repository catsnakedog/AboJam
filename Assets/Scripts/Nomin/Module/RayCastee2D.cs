using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastee2D : MonoBehaviour
{
    /* Field & Property */
    public static List<RayCastee2D> instances = new List<RayCastee2D>();

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    public void OnClick()
    {
        Debug.Log($"{name} 이 클릭되었습니다.");
    }
}
