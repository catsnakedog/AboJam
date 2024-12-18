using UnityEngine;

public class Zoom : MonoBehaviour
{
    public Camera cam;
    private float speed_zoom = 3;
    private float min_zoom = 5;
    private float max_zoom = 8;
    private float time_smooth = 0.2f;

    private float target_zoom;
    private float speed_zoom_before;

    void Start()
    {
        target_zoom = cam.orthographicSize;
    }

    void Update()
    {
        // PC (���콺 ��)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            target_zoom -= scroll * speed_zoom;
            target_zoom = Mathf.Clamp(target_zoom, min_zoom, max_zoom);
        }

        // ����� (�� �հ��� ��ġ)
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // �� �հ��� ���� �Ÿ� ���� ��� (Zoom �� / ��)
            float prevDistance = (touch1.position - touch1.deltaPosition - (touch2.position - touch2.deltaPosition)).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;
            float distance = prevDistance - currentDistance;

            target_zoom += distance * speed_zoom * 0.01f;
            target_zoom = Mathf.Clamp(target_zoom, min_zoom, max_zoom);
        }

        // �ε巯�� ��ȯ
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, target_zoom, ref speed_zoom_before, time_smooth);
    }
}
