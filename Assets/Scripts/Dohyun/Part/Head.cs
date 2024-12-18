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
        // 1. 마우스의 화면 좌표를 가져옴
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. 마우스의 월드 좌표를 계산 (카메라 기준 Z 깊이 설정)
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
        mouseWorldPosition.z = 0; // 2D 환경에서는 Z축 고정

        // 3. 오브젝트의 위치에서 마우스 위치로 향하는 방향 벡터 계산
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. 방향 벡터를 기준으로 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. 오브젝트의 자체 Z축 기준 회전 추가 (90도)
        float adjustedAngle = angle + 180;

        // 6. 좌측/우측 판별하여 Flip 처리
        Vector3 localScale = obj.transform.localScale;
        if (direction.x > 0) // 우측 방향
        {
            localScale.y = -1;
            if (adjustedAngle < 155)
                adjustedAngle = 155;
            if (adjustedAngle > 205)
                adjustedAngle = 205;
        }
        else // 좌측 방향
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

        // 7. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }
}
