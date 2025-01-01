using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    private Camera mainCamera;
    public TextMeshProUGUI Abocado;
    public TextMeshProUGUI Garu;
    public GameObject HP;

    private void Start()
    {
        StartCoroutine(CorUpdate());

        // 해상도 고정
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);

        // 카메라 설정

        mainCamera = Camera.main;
        MaintainAspectRatio();
    }

    public IEnumerator CorUpdate()
    {
        while (true)
        {
            Abocado.text = StaticData.Abocado.ToString();
            Garu.text = StaticData.Garu.ToString();

            yield return new WaitForSeconds(0.1f);
        }
    }

    void MaintainAspectRatio()
    {
        float targetAspect = 16.0f / 9.0f;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Letterbox
            Rect rect = mainCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            mainCamera.rect = rect;
        }
        else
        {
            // Pillarbox
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = mainCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            mainCamera.rect = rect;
        }
    }
}
