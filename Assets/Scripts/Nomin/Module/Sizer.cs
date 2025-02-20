using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트의 크기와 위치를 클라이언트 화면과 동기화합니다.
/// </summary>
public class Sizer : MonoBehaviour
{
    [SerializeField] GameObject[] objects; // 자동으로 화면에 맞게 조절 될 오브젝트 추가
    [SerializeField] float resolutionWidth; // 에디터 기준 해상도
    [SerializeField] float resolutionHeight; // 에디터 기준 해상도

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        foreach (GameObject obj in objects) Size(obj);
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
