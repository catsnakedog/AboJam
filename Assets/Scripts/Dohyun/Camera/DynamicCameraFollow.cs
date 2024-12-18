using UnityEngine;

public class DynamicCameraFollow : MonoBehaviour
{
    public Transform Player; // �÷��̾��� Transform
    public RectTransform CameraBox; // CameraBox�� Transform
    public RectTransform InsideBox; // InsideBox�� Transform
    public RectTransform OutsideBox; // OutSideBox�� Transform
    public float SmoothSpeed = 0.125f; // ī�޶� �ε巯�� �̵� �ӵ�

    private Camera _mainCamera;
    private float _fixedZ; // ī�޶� Z�� ���� ��

    void Start()
    {
        _mainCamera = Camera.main; // ���� ī�޶� ����
        _fixedZ = transform.position.z; // �ʱ� Z�� �� ����
    }

    void FixedUpdate()
    {
        SetWorldPosition();
    }

    private void SetWorldPosition()
    {
        float smooth = SmoothSpeed;

        // ȭ�� �߾� ��ǥ ��� (��ũ�� ����)
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0);

        // ���콺 ��ǥ�� ȭ�� �߽� ���� ��� ��ǥ�� ��ȯ
        Vector3 mouseRelativeToCenter = Input.mousePosition - screenCenter;
        Vector3 screenPosition = CalculateScreenPosition(mouseRelativeToCenter);

        if (screenPosition == Vector3.zero)
            smooth = 0.25f;

        // ��� ��ǥ�� ���� �������� ��ȯ
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
        // ī�޶��� FOV�� ȭ�� ���� ��������
        return distance / ((Screen.height * 2) /_mainCamera.orthographicSize);
    }
}
