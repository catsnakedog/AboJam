using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    /* Dependency */
    public Collider2D colider2D;

    /* Field & Property */
    public UnityEvent unityEvent;

    /* Private Method */
    private void OnMouseDown()
    {
        Debug.Log($"{name} 이 클릭됨");
        unityEvent.Invoke();
    }
}
