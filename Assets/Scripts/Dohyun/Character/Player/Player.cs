using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public PlayerMovement playerMovement;
    public Hand Hand;
    public Head Head;

    void Awake()
    {
        Movement = playerMovement;
        Hand?.Init();
        Head?.Init();
        Init();
    }

    private void Update()
    {
        playerMovement.MoveAction?.Invoke();
        Hand.HandAction?.Invoke();
        Head.HeadAction?.Invoke();
    }

    public Coroutine StartCoroutineHelper(IEnumerator target)
    {
        return StartCoroutine(target);
    }

    public void StopCoroutineHelper(Coroutine target)
    {
        StopCoroutine(target);
    }
}