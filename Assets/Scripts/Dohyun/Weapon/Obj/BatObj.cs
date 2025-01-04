using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatObj : MonoBehaviour
{
    public bool AttackFlag;

    public void Attack()
    {
        AttackFlag = true;
    }
}
