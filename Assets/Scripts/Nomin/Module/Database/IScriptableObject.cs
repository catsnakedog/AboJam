using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScriptableObject<T> where T : ScriptableObject
{
    T SO { get; set; }

    /// <summary>
    /// <br>스크립터블 오브젝트의 값을 불러옵니다.</br>
    /// <br>즉, 모든 하위 요소를 초기값으로 되돌립니다.</br>
    /// </summary>
    void Load();
}
