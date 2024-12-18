using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tiled : MonoBehaviour
{
    public GameObject tiled;

    public void Start()
    {
        // ��� Map ���� ũ�� ����
        foreach (Transform map in tiled.transform)
        {
            // map.localScale = new Vector2(91, 91);

            // ��� Grid ���� ��ġ ����
            foreach (Transform grid in map)
            {
                // grid.localPosition = new Vector2(-8.8f, 5.93f);

                // BackGround �Ǵ� ForeGround
                foreach (Transform layerGroup in grid)
                {
                    // BackGround �� �� ���̾� ���� Sorting Layer ����
                    if (layerGroup.gameObject.name == "BackGround")
                    {
                        foreach (Transform layer in layerGroup)
                        {
                            layer.GetComponent<TilemapRenderer>().sortingLayerName = "BackGround";
                        }
                    }

                    // ForeGround �� �� ���̾� ���� Sorting Layer ����
                    if (layerGroup.gameObject.name == "ForeGround")
                    {
                        foreach (Transform layer in layerGroup)
                        {
                            layer.GetComponent<TilemapRenderer>().sortingLayerName = "ForeGround";
                        }
                    }
                }
            }
        }
    }
}
