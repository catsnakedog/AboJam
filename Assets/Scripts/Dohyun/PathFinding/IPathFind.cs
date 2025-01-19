using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFind
{
    public void MakeRoute(Vector3 charaterPos);

    public Vector2 GetDirection(Vector3 charaterPos);
}
