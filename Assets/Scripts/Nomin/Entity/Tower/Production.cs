using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : Tower
{
    /* Dependency */
    public Abocado abocado;

    /* Field & Property */
    public static List<Production> instances = new List<Production>();

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        //instances.Add(this);

        // 아래 코드가 abocado 의 Start 보다 먼저 실행되어서 문제가 생김.
        abocado.harvest++;
        abocado.GrowUp(true);
        abocado.GrowUp();
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
}
