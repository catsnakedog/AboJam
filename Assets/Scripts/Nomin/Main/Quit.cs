using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{
    /// <summary>
    /// 게임을 종료합니다.
    /// </summary>
    public void Shutdown()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Shutdown();
    }
}
