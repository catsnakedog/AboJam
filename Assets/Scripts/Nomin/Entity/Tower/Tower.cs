using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    /* Dependency */
    public HP hp;
    public Launcher launcher;

    /* Field & Property */
    public static List<Tower> instances = new List<Tower>(); // 모든 타워 인스턴스

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
