using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    /// <summary>
    /// 게임을 종료합니다.
    /// </summary>
    public void Shutdown()
    {
        Application.Quit();
    }
}
