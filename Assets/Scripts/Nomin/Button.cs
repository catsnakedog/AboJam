using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject abocade;

    public void Switch()
    {
        Grid.GetTile(1, 1).SwitchWall(true);
    }
}
