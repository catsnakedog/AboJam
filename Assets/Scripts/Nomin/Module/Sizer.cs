using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 오브젝트의 크기와 위치를 클라이언트 화면과 동기화합니다.
/// </summary>
public class Sizer : MonoBehaviour
{
    /* Dependency */
    [SerializeField] CanvasScaler cameraRectCanvasScaler;
    [SerializeField] float resolutionWidth; // 에디터 기준 해상도
    [SerializeField] float resolutionHeight; // 에디터 기준 해상도
#if UNITY_STANDALONE_WIN
    // 플랫폼이 윈도우일 경우, 애플리케이션 창 크기를 조절 중 인지 검사합니다.
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
    private const int VK_LBUTTON = 0x01; // 마우스 왼쪽 버튼
    private bool IsResizing()
    {
        if (GetAsyncKeyState(VK_LBUTTON) != 0) return true;
        else return false;
    }
#endif

    /* Field & Property */
    int lastWidth = 0;
    int lastHeight = 0;

    /* Initializer & Finalizer & Updater */
    private void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
#if UNITY_STANDALONE_WIN
            if (IsResizing()) return; // 윈도우 창 크기 조절이 끝나고 수행
#endif

            UpdateScreenRatio(16, 9);
            UpdateCanvasScaler();
        }
    }
    /// <summary>
    /// 화면 비율을 width : height 로 변경합니다.
    /// </summary>
    private void UpdateScreenRatio(float width, float height)
    {
        // 가로 변동 시 비율에 맞춰 세로 조절
        if (lastWidth != Screen.width)
        {
            lastWidth = Screen.width;
            lastHeight = Mathf.RoundToInt((height / width) * Screen.width);
            Screen.SetResolution(lastWidth, lastHeight, false);
        }
        // 세로 변동 시 비율에 맞춰 가로 조절
        else if (lastHeight != Screen.height)
        {
            lastHeight = Screen.height;
            lastWidth = Mathf.RoundToInt((width / height) * Screen.height);
            Screen.SetResolution(lastWidth, lastHeight, false);
        }
    }
    /// <summary>
    /// CameraRect 의 캔버스 스케일러를 조정합니다.
    /// </summary>
    private void UpdateCanvasScaler()
    {
        cameraRectCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cameraRectCanvasScaler.referenceResolution = new Vector2(resolutionWidth, resolutionHeight);
        cameraRectCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cameraRectCanvasScaler.matchWidthOrHeight = 0;
    }
}
