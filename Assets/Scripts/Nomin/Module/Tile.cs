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
    /* Field & Property */
    public static List<Tile> instances = new List<Tile>(); // 모든 타일 인스턴스
    public static Tile currentTile;
    public GameObject Go { get; set; } = null; // 타일에 배치된 프리팹
    public readonly int i;
    public readonly int j;
    public Vector2 pos;
    public bool isWall { get; private set; } = false;
    private string path_abocado = "Prefabs/Entity/Abocado";

    /* Intializer & Finalizer & Updater */
    public Tile(int i, int j, Vector2 pos)
    {
        instances.Add(this);
        this.i = i;
        this.j = j;
        this.pos = pos;
    }

    /* Public Method */
    /// <summary>
    /// <br>타일을 클릭했을 때 발생하는 이벤트 입니다.</br>
    /// <br>설치된 오브젝트로 이벤트가 구분됩니다.</br>
    /// </summary>
    public void OnClick()
    {
        Debug.Log($"타일 [" + i + "][" + j + "] 이 클릭되었습니다.");
        Promotion.instance.gameObject.SetActive(false);

        // 오브젝트 컨트롤 모드 (F) : 오브젝트 Go 에 따라 다른 행동을 합니다.
        if (Input.GetKey(KeyCode.F))
        {
            switch (Go)
            {
                // NULL : Abocado 프리팹 건설
                case null:
                    Create(Resources.Load<GameObject>(path_abocado));
                    return;
            }
        }
    }
    /// <summary>
    /// 타일의 이동 가능 / 불가 속성을 스위칭합니다.
    /// </summary>
    /// <param name="b">BoxColider2D.active</param>
    public void SwitchWall(bool b)
    {
        //isWall = b;
        //bocCollider2D.enabled = b;
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
        Go = null;
    }
}