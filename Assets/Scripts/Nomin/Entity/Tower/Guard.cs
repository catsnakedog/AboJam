using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Tower
{
    /* Field & Property */
    public static List<Guard> instances = new List<Guard>(); // 모든 방어 타워 인스턴스

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
}
