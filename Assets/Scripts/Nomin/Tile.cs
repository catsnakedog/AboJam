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
        // gameObject.name ���κ��� index ����
        Match match = Regex.Match(gameObject.name, @"\d+");
        if (match.Success) { index = int.Parse(match.Value); }
        else { index = 0; }
        // index ���κ��� i, j ����
        (i, j) = Grid.ConvertIndexToArray(index);
        col = GetComponent<BoxCollider2D>();
        upgrade = FindInactiveByTag("Upgrade");
    }

    public void OnClick()
    {
        Debug.Log($"[" + i + "][" + j + "]\n" + $"Index : {index}");
        upgrade.SetActive(false);

        // ���� ��Ʈ�� ��� (F)
        if (Input.GetKey(KeyCode.F))
        {
            // �� Ÿ��
            if (go == null)
            {
                // �ƺ�ī�� ����
                go = Resources.Load<GameObject>("Prefabs/Abocado");
                Abocado abc = go.GetComponent<Abocado>();
                Create(go);
            }
            // ä���� Ÿ��
            else
            {
                Abocado abc = go.GetComponent<Abocado>();
                if (abc == null) return;

                go.gameObject.GetComponent<Animation_Click>().OnClick();

                // Level 0 = �ɱ�
                if (abc.level == 0 && StaticData.Abocado > 0)
                {
                    StaticData.Abocado--;
                    abc.LevelUp(true);
                }
                // Level 2 = Ÿ�� ���׷��̵�
                else if (abc.level == 2)
                {
                    currentTile = this;
                    upgrade.SetActive(true);
                }
                // Level 3 = ��Ȯ
                else if (abc.level == 3) abc.Harvest();
            }
        }
    }

    public void SwitchWall(bool b)
    {
        isWall = b;
        col.enabled = b;
    }

    /// <summary>
    /// ���� Ÿ�� ��ġ�� ������ �������� �����մϴ�.
    /// </summary>
    public void Create(GameObject go)
    {
        this.go = Instantiate(go, gameObject.transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Ÿ�Ͽ� �����ϴ� �������� �����մϴ�.
    /// </summary>
    public void Delete()
    {
        Destroy(go);
    }

    /// <summary>
    /// <br>Equals �� ������ �� ��, �ؽ� �ڵ嵵 ������ �ؾ��մϴ�.</br>
    /// <br>�̴� �ؽ� ��� �÷��ǿ����� �ùٸ��� Ž���� �� �ֵ��� �մϴ�.</br>
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