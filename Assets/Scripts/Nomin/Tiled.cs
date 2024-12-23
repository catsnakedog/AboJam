using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tiled : MonoBehaviour
{
    public GameObject tiled;

    public void Start()
    {
        // 모든 Map 마다 크기 조절
        foreach (Transform map in tiled.transform)
        {
            // map.localScale = new Vector2(91, 91);

            // 모든 Grid 마다 위치 정렬
            foreach (Transform grid in map)
            {
                // grid.localPosition = new Vector2(-8.8f, 5.93f);

                // BackGround 또는 ForeGround
                foreach (Transform layerGroup in grid)
                {
                    // BackGround 의 각 레이어 마다 Sorting Layer 변경
                    if (layerGroup.gameObject.name == "BackGround")
                    {
                        foreach (Transform layer in layerGroup)
                        {
                            layer.GetComponent<TilemapRenderer>().sortingLayerName = "BackGround";
                        }
                    }

                    // ForeGround 의 각 레이어 마다 Sorting Layer 변경
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
