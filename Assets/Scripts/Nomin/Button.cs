using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject abocade;

    public void ON()
    {
        Grid.GetBlock(1, 1).Create(abocade);
    }
}
