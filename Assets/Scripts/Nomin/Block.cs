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
    private BoxCollider2D col;

    public void Start()
    {
        // gameObject.name ���κ��� index ����
        Match match = Regex.Match(gameObject.name, @"\d+");
        if (match.Success) { index = int.Parse(match.Value); }
        else { index = 0; }
        // index ���κ��� i, j ����
        (i, j) = Grid.ConvertIndexToArray(index);
        col = GetComponent<BoxCollider2D>();

    }

    public void OnClick()
    {
        Debug.Log($"[" + i + "][" + j + "]\n" + $"Index : {index}");

        // ��Ȯ & �ɱ� ��� (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // �� Ÿ��
            if (go == null)
            {

            }
            // ä���� Ÿ��
            else
            {
                Abocado abc = go.GetComponent<Abocado>();

                // �ƺ�ī�� ��Ȯ
                if (abc != null) abc.Harvest();
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
        this.go = go;
        Instantiate(go, gameObject.transform.position, Quaternion.identity);
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
}