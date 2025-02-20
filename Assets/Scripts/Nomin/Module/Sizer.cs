using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 오브젝트의 크기와 위치를 클라이언트 화면과 동기화합니다.
/// </summary>
public class Sizer : MonoBehaviour
{
    /* Dependency */
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
    [SerializeField] GameObject[] objects; // 자동으로 화면에 맞게 조절 될 오브젝트 추가
    [SerializeField] float resolutionWidth; // 에디터 기준 해상도
    [SerializeField] float resolutionHeight; // 에디터 기준 해상도
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

            SetScreenRatio(16, 9);
            foreach (GameObject obj in objects) Size(obj);
        }
    }

    /* Public Method */
    /// <summary>
    /// GameObject 의 크기와 위치를 화면 비율에 따라 조정합니다.
    /// </summary>
    public void Size(GameObject go)
    {
        ScaleWithScreenSize(go);
        PositionWithScreenSize(go);
    }
    /// <summary>
    /// 화면 비율을 width : height 로 변경합니다.
    /// </summary>
    public void SetScreenRatio(float width, float height)
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

    /* Private Method */
    /// <summary>
    /// 게임 오브젝트의 스케일을 화면 비율에 따라 조정합니다.
    /// </summary>
    private void ScaleWithScreenSize(GameObject go)
    {
        go.transform.localScale *= (Screen.width / resolutionWidth);
    }
    /// <summary>
    /// 게임 오브젝트의 포지션을 화면 비율에 따라 조정합니다.
    /// </summary>
    private void PositionWithScreenSize(GameObject go)
    {
        float newPosX = go.transform.localPosition.x * (Screen.width / resolutionWidth);
        float newPosY = go.transform.localPosition.y * (Screen.height / resolutionHeight);

        go.transform.localPosition = new Vector3(newPosX, newPosY, go.transform.localPosition.z);
    }
}
