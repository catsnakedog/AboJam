using UnityEngine;

public class DynamicCameraFollow : MonoBehaviour
{
    public Transform Player; // 플레이어의 Transform
    public RectTransform CameraBox; // CameraBox의 Transform
    public RectTransform InsideBox; // InsideBox의 Transform
    public RectTransform OutsideBox; // OutSideBox의 Transform
    public float SmoothSpeed = 0.125f; // 카메라 부드러운 이동 속도

    private Camera _mainCamera;
    private float _fixedZ; // 카메라 Z축 고정 값

    void Start()
    {
        _mainCamera = Camera.main; // 메인 카메라 참조
        _fixedZ = transform.position.z; // 초기 Z축 값 저장
        Player = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        SetWorldPosition();
    }

    private void SetWorldPosition()
    {
        float smooth = SmoothSpeed;

        // 카메라와 Aim 을 연동시킵니다.
        Vector2 inputDirection = Receiver.instance.aimDirection;
        float inputMagnitude = Receiver.instance.aimMagnitude;

        Vector3 stickRelativeToCenter = new Vector3(
            inputDirection.x * (OutsideBox.sizeDelta.x / 2f),
            inputDirection.y * (OutsideBox.sizeDelta.y / 2f),
            0 );
        Vector3 screenPosition = CalculateScreenPosition(stickRelativeToCenter) * inputMagnitude;

        /* Aim 방향을 마우스와 연동시킵니다.
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 mouseRelativeToCenter = Input.mousePosition - screenCenter;
        Vector3 screenPosition = CalculateScreenPosition(mouseRelativeToCenter);
        */

        if (screenPosition == Vector3.zero) smooth = 0.25f;

        // 상대 좌표를 월드 공간으로 변환
        Vector3 relativeToWorld = new(GetPixelsPerWorldUnit(screenPosition.x), GetPixelsPerWorldUnit(screenPosition.y), _fixedZ);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, Player.position + relativeToWorld, smooth);
        transform.position = smoothedPosition;
    }

    private Vector3 CalculateScreenPosition(Vector3 position)
    {
        if((Mathf.Abs(position.x) < Mathf.Abs(InsideBox.sizeDelta.x/2)) && (Mathf.Abs(position.y) < Mathf.Abs(InsideBox.sizeDelta.y/2)))
        {
            return Vector3.zero;
        }
        return new Vector3(position.x / OutsideBox.sizeDelta.x * CameraBox.sizeDelta.x, position.y / OutsideBox.sizeDelta.y * CameraBox.sizeDelta.y, _fixedZ); 
    }

    private float GetPixelsPerWorldUnit(float distance)
    {
        // 카메라의 FOV와 화면 높이 가져오기
        return distance / ((Screen.height * 2) /_mainCamera.orthographicSize);
    }
}
