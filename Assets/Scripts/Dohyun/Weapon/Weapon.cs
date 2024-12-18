using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weaponPrefab; // 무기 프리팹

    private GameObject equippedWeapon; // 장착된 무기

    public virtual void InitializeWeapon(Vector2 weaponPlace)
    {
        // 무기 생성 및 장착
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

        // 마우스 위치 가져오기
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // 무기가 마우스를 향하도록 회전
        Vector2 direction = (mousePosition - equippedWeapon.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        equippedWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}