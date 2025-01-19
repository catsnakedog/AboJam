using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Database
{
    public struct Table_HP
    {
        public string HPID;
        public float Hp_max;
        public bool HideFullHP;
    }

    public Table_HP[] HP;
}
