using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public Movement Movement; // 이동 관리
    [HideInInspector]
    public Weapon Weapon; // 무기 관리
    public Vector2 WeaponPlace; // 무기 위치 (로컬 좌표)

    public virtual void Init()
    {
        // 이동 초기화
        Movement?.InitMovement(gameObject.GetComponent<Rigidbody2D>());

        // 무기 초기화
        Weapon?.InitializeWeapon(WeaponPlace);
    }
}