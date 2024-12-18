using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Weapon;

[System.Serializable]
public class Head
{
    public Action HeadAction;
    public GameObject Player;
    public GameObject HeadObj;

    public void Init()
    {
        HeadAction = HeadSet;
    }

    public void HeadSet()
    {
        HeadObj.transform.rotation = ForwardToMouse(HeadObj);
    }

    public Quaternion ForwardToMouse(GameObject obj)
    {
        // 1. ���콺�� ȭ�� ��ǥ�� ������
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. ���콺�� ���� ��ǥ�� ��� (ī�޶� ���� Z ���� ����)
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
        mouseWorldPosition.z = 0; // 2D ȯ�濡���� Z�� ����

        // 3. ������Ʈ�� ��ġ���� ���콺 ��ġ�� ���ϴ� ���� ���� ���
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. ���� ���͸� �������� ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. ������Ʈ�� ��ü Z�� ���� ȸ�� �߰� (90��)
        float adjustedAngle = angle + 180;

        // 6. ����/���� �Ǻ��Ͽ� Flip ó��
        Vector3 localScale = obj.transform.localScale;
        if (direction.x > 0) // ���� ����
        {
            localScale.y = -1;
            if (adjustedAngle < 155)
                adjustedAngle = 155;
            if (adjustedAngle > 205)
                adjustedAngle = 205;
        }
        else // ���� ����
        {
            localScale.y = 1;
            if(adjustedAngle > 25 && adjustedAngle < 180)
                adjustedAngle = 25;
            else if(adjustedAngle > 25 && adjustedAngle > 180)
                adjustedAngle = 335;
            else if (adjustedAngle > 335 && adjustedAngle > 25)
                adjustedAngle = 335;
        }
        obj.transform.localScale = localScale;

        // 7. ���� ȸ���� ��ȯ
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }
}
