using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weaponPrefab; // ���� ������

    private GameObject equippedWeapon; // ������ ����

    public virtual void InitializeWeapon(Vector2 weaponPlace)
    {
        // ���� ���� �� ����
        if (weaponPrefab != null)
        {
            Transform parentTransform = transform;
            Vector3 weaponPosition = parentTransform.position + (Vector3)weaponPlace;
            equippedWeapon = Instantiate(weaponPrefab, weaponPosition, Quaternion.identity, parentTransform);
        }
    }

    public void RotateWeaponToMouse()
    {
        if (equippedWeapon == null) return;

        // ���콺 ��ġ ��������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // ���Ⱑ ���콺�� ���ϵ��� ȸ��
        Vector2 direction = (mousePosition - equippedWeapon.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        equippedWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}