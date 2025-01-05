using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RayCastee2D : MonoBehaviour
{
    /* Dependency */
    public Collider2D collider2D;

    /* Field & Property */
    public static List<RayCastee2D> instances = new List<RayCastee2D>();
    public UnityEvent unityEvent;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* public Method */
    public void OnClick()
    {
        unityEvent.Invoke();
    }
}
