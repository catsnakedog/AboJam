using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = new();
        Movement = playerMovement;
        Init();
    }

    private void Update()
    {
        playerMovement.MoveAction?.Invoke();
    }
}