using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    /* Dependency */
    [Header("Dependency")]
    public Button button;
    public BoxCollider2D bocCollider2D;
    private GameObject upgrade;

    /* Field */
    public static Tile currentTile; // 마지막으로 클릭한 타일
    public GameObject Go { get; set; } = null; // 타일에 배치된 프리팹
    public int Index { get; private set; } // { get; private set; } = 외부 readonly
    public int I { get; private set; }
    public int J { get; private set; }
    private bool isWall = false;
    private string path_abocado = "Prefabs/Abocado";

    /* Intializer & Finalizer */
    public void Start()
    {
        // gameObject.name 으로부터 Index 추출
        Match match = Regex.Match(gameObject.name, @"\d+");
        if (match.Success) { Index = int.Parse(match.Value); }
        else { Debug.Log($"타일 {gameObject.name} 의 이름에서 Index 추출에 실패하였습니다."); return; /*Index = 0;*/ }

        // Index 으로부터 I, J 추출
        (I, J) = Grid.ConvertIndexToArray(Index);
        upgrade = Upgrade.instance.gameObject;
    }

    /* Public Method */
    /// <summary>
    /// <br>타일을 클릭했을 때 발생하는 이벤트 입니다.</br>
    /// </summary>
    public void OnClick()
    {
        Debug.Log($"[" + I + "][" + J + "]\n" + $"Index : {Index}");
        upgrade.SetActive(false);

        // 오브젝트 컨트롤 모드 (F) : 오브젝트 Go 에 따라 다른 행동을 합니다.
        if (Input.GetKey(KeyCode.F))
        {
            switch (Go)
            {
                // NULL : Abocado 프리팹 건설
                case null:
                    Create(Resources.Load<GameObject>(path_abocado));
                    return;
                // 아보카도 : 레벨에 따라 성장 & 수확
                case { } when Go.GetComponent<Abocado>() != null:
                    Abocado abc = Go.GetComponent<Abocado>();
                    switch (abc.level)
                    {
                        // Cultivated : Seed 로 성장
                        case EnumData.Abocado.Cultivated:
                            if (StaticData.Abocado > 0)
                            {
                                StaticData.Abocado--;
                                abc.GrowUp(true);
                            }
                            break;
                        // Tree : 타워 업그레이드 패널 On
                        case EnumData.Abocado.Tree:
                            currentTile = this;
                            upgrade.SetActive(true);
                            break;
                        // Fruited : 수확
                        case EnumData.Abocado.Fruited:
                            abc.Harvest();
                            break;
                    }
                    break;
                // 타워 : 
                // case { } when Go.GetComponent<Tower>() != null:
            }

            // 클릭 애니메이션 재생
            AnimationClick animationClick = Go.GetComponent<AnimationClick>();
            if (animationClick != null) animationClick.OnClick();
        }
    }
    /// <summary>
    /// 타일의 이동 가능 / 불가 속성을 스위칭합니다.
    /// </summary>
    /// <param name="b">BoxColider2D.active</param>
    public void SwitchWall(bool b)
    {
        isWall = b;
        bocCollider2D.enabled = b;
    }
    /// <summary>
    /// 현재 타일 위치에 지정한 프리팹을 생성합니다.
    /// </summary>
    public void Create(GameObject Go)
    {
        if (this.Go != null) { Debug.Log($"{gameObject.name} 에 이미 {this.Go.name} 가 건설되어 있습니다."); return; }
        this.Go = Instantiate(Go, gameObject.transform.position, Quaternion.identity);
    }
    /// <summary>
    /// 타일에 존재하는 프리팹을 제거합니다.
    /// </summary>
    public void Delete()
    {
        Destroy(Go);
    }
}