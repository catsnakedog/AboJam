using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public Movement Movement; // �̵� ����
    [HideInInspector]
    public Weapon Weapon; // ���� ����
    public Vector2 WeaponPlace; // ���� ��ġ (���� ��ǥ)

    public virtual void Init()
    {
        // �̵� �ʱ�ȭ
        Movement?.InitMovement(gameObject.GetComponent<Rigidbody2D>());

        // ���� �ʱ�ȭ
        Weapon?.InitializeWeapon(WeaponPlace);
    }
}