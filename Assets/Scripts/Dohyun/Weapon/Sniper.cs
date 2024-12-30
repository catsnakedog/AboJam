using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : RangedWeapon
{
    public override void AttackLogic()
    {
        var obj = ObjectPool.Instance.GetObj(Bullet, BulletObj);
        var rotation = transform.parent.rotation; // �θ��� ȸ�� ��������
        float spreadAngle = UnityEngine.Random.Range(-WeaponDatas[Level - 1].Spread, WeaponDatas[Level - 1].Spread);

        // �θ� ���⿡ ���� ������ �߰��� ȸ�� ���
        Quaternion spreadRotation = rotation * Quaternion.Euler(0, 0, spreadAngle - 90);

        // ��������� źȯ�� ���� ����
        obj.transform.rotation = spreadRotation;
        obj.transform.position = FireLocation.position;
        float angleInDegrees = spreadRotation.eulerAngles.z;
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // 2. XY ��鿡���� ���� ���� ���
        Vector3 direction = new Vector3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0);

        obj.GetComponent<Bullet>().Init(spreadRotation, direction, WeaponDatas[Level - 1].Damage, WeaponDatas[Level - 1].bulletPenetration, WeaponDatas[Level - 1].Range, WeaponDatas[Level - 1].BulletSpeed);
    }
}
