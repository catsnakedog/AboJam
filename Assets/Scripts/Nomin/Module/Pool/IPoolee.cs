using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolee
{
    /// <summary>
    /// Get 시 오브젝트를 초기화하기 위한 코드를 작성합니다.
    /// </summary>
    void Load();

    /// <summary>
    /// Return 시 데이터를 저장하기 위한 코드를 작성합니다.
    /// </summary>
    void Save();
}
