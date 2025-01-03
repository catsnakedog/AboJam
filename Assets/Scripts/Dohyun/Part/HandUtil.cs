using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hand;
using static Weapon;

public static class HandUtil
{
    public static Quaternion ForwardToObj(GameObject obj1, Vector3 obj2, float handAngleCorrection)
    {
        // 두 번째 오브젝트를 향하는 방향 벡터 계산
        Vector3 direction = obj2 - obj1.transform.position;
        direction.z = 0; // 2D 환경에서는 Z축 무시
        if (direction == Vector3.zero) return Quaternion.identity; // 방향이 없으면 기본 회전 반환

        // 방향 벡터를 기준으로 회전 계산 (Z축 회전만)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float adjustedAngle = angle + handAngleCorrection;

        // 6. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }

    public static Vector3 ScreenToWorld2D(Vector3 screenPosition, Camera mainCamera)
    {
        // 1. 화면 좌표를 0~1로 정규화
        float normalizedX = screenPosition.x / Screen.width;
        float normalizedY = screenPosition.y / Screen.height;

        // 2. 카메라의 월드 좌표 범위 계산
        float halfHeight = mainCamera.orthographicSize; // 카메라의 세로 절반 크기
        float halfWidth = halfHeight * mainCamera.aspect; // 카메라의 가로 절반 크기 (aspect 비율 고려)

        // 3. 월드 좌표 계산
        float worldX = Mathf.Lerp(mainCamera.transform.position.x - halfWidth,
                                  mainCamera.transform.position.x + halfWidth,
                                  normalizedX);

        float worldY = Mathf.Lerp(mainCamera.transform.position.y - halfHeight,
                                  mainCamera.transform.position.y + halfHeight,
                                  normalizedY);

        // 4. Z축 깊이는 0
        float worldZ = 0f;

        return new Vector3(worldX, worldY, worldZ);
    }

    public static Quaternion ForwardToMouse(GameObject obj, Camera mainCamera, float handAngleCorrection)
    {
        // 1. 마우스의 화면 좌표를 가져옴
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. 마우스의 월드 좌표를 계산 (카메라 기준 Z 깊이 설정)
        Vector3 mouseWorldPosition = ScreenToWorld2D(mouseScreenPosition, mainCamera);

        // 3. 오브젝트의 위치에서 마우스 위치로 향하는 방향 벡터 계산
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. 방향 벡터를 기준으로 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. 오브젝트의 자체 Z축 기준 회전 추가 (90도)
        float adjustedAngle = angle + handAngleCorrection;

        // 7. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }

    public static Quaternion RimitedForwardToMouse(GameObject obj, Camera mainCamera, float maxAngle, float minAngle, float correctAngle, bool adjustFlip = false)
    {
        // 1. 마우스의 화면 좌표를 가져옴
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. 마우스의 월드 좌표를 계산 (카메라 기준 Z 깊이 설정)
        Vector3 mouseWorldPosition = ScreenToWorld2D(mouseScreenPosition, mainCamera);

        // 3. 오브젝트의 위치에서 마우스 위치로 향하는 방향 벡터 계산
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. 방향 벡터를 기준으로 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. 각도 계산 후 각도 범위 제한 처리
        if (angle >= 0)
        {
            if (angle > maxAngle && angle < 90) angle = maxAngle;
            if (angle < 180 - maxAngle && angle > 90) angle = 180 - maxAngle;
        }
        else
        {
            if (angle < -minAngle && angle > -90) angle = -minAngle;
            if (angle > -180 + minAngle && angle < -90) angle = -180 + minAngle;
        }

        // 6. 좌측/우측 판별하여 Flip 처리
        if (adjustFlip)
        {
            int y;
            if (direction.x > 0) y = -1;
            else y = 1;
            obj.transform.localScale = new Vector3(1, y, 1);
        }

        // 7. 값 조정
        float adjustedAngle = angle + correctAngle;

        if (!adjustFlip) Debug.Log(angle);
        // 8. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }

    public static bool ApplyHandSwap(WeaponHandState weaponHandState, HandType type, bool isFlip)
    {
        return type switch
        {
            HandType.Standard => isFlip ? !weaponHandState.StandardHand : weaponHandState.StandardHand,
            HandType.Left => isFlip ? weaponHandState.RightUse : weaponHandState.LeftUse,
            _ => isFlip ? weaponHandState.LeftUse : weaponHandState.RightUse,
        };
    }

    public static bool IsRightHandStandard(Weapon weapon, bool changeHand)
    {
        return ApplyHandSwap(weapon.HandState, HandType.Standard, changeHand);
    }
}
