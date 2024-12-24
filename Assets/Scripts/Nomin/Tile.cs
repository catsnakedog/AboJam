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
    public int index;
    public int i, j;
    public bool isWall { get; set; } = false;
    public GameObject go { get; set; } = null;
    public GameObject upgrade;
    private BoxCollider2D col;
    public static Tile currentTile;

    public void Start()
    {
        // gameObject.name 으로부터 index 추출
        Match match = Regex.Match(gameObject.name, @"\d+");
        if (match.Success) { index = int.Parse(match.Value); }
        else { index = 0; }
        // index 으로부터 i, j 추출
        (i, j) = Grid.ConvertIndexToArray(index);
        col = GetComponent<BoxCollider2D>();
        upgrade = FindInactiveByTag("Upgrade");
    }

    public void OnClick()
    {
        Debug.Log($"[" + i + "][" + j + "]\n" + $"Index : {index}");
        upgrade.SetActive(false);

        // 나무 컨트롤 모드 (F)
        if (Input.GetKey(KeyCode.F))
        {
            // 빈 타일
            if (go == null)
            {
                // 아보카도 경작
                go = Resources.Load<GameObject>("Prefabs/Abocado");
                Abocado abc = go.GetComponent<Abocado>();
                Create(go);
            }
            // 채워진 타일
            else
            {
                Abocado abc = go.GetComponent<Abocado>();
                if (abc == null) return;

                go.gameObject.GetComponent<AnimationClick>().OnClick();

                // Cultivate > Seed
                if (abc.level == EnumData.Abocado.Cultivated && StaticData.Abocado > 0)
                {
                    StaticData.Abocado--;
                    abc.GrowUp(true);
                }
                // Tree > Upgrade
                else if (abc.level == EnumData.Abocado.Tree)
                {
                    currentTile = this;
                    upgrade.SetActive(true);
                }
                // Fruited > Harvest
                else if (abc.level == EnumData.Abocado.Fruited) abc.Harvest();
            }
        }
    }

    public void SwitchWall(bool b)
    {
        isWall = b;
        col.enabled = b;
    }

    /// <summary>
    /// 현재 타일 위치에 지정한 프리팹을 생성합니다.
    /// </summary>
    public void Create(GameObject go)
    {
        this.go = Instantiate(go, gameObject.transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 타일에 존재하는 프리팹을 제거합니다.
    /// </summary>
    public void Delete()
    {
        Destroy(go);
    }

    /// <summary>
    /// <br>Equals 를 재정의 할 때, 해시 코드도 재정의 해야합니다.</br>
    /// <br>이는 해시 기반 컬렉션에서도 올바르게 탐색할 수 있도록 합니다.</br>
    /// </summary>
    public override int GetHashCode()
    {
        return (i, j).GetHashCode();
    }

    public GameObject FindInactiveByTag(string tag)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();

        foreach (Transform t in allTransforms)
        {
            if (t.gameObject.CompareTag(tag) && !t.gameObject.activeSelf)
            {
                return t.gameObject;
            }
        }

        return null;
    }
}