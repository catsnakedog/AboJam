using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private IPlayerState _currentState;
    private List<IObserver> _observers = new();

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

    public void ChangeState(IPlayerState newState)
    {
        _currentState?.Exit(this);  // 기존 상태 종료
        _currentState = newState;  // 새로운 상태 설정
        _currentState.Enter(this); // 새로운 상태 진입
    }

    public void AddObserver(IObserver observer) => _observers.Add(observer);
    public void RemoveObserver(IObserver observer) => _observers.Remove(observer);

    // 상태 변경 알림
    public void NotifyObservers(string state)
    {
        foreach (var observer in _observers)
        {
            observer.OnNotify(state);
        }
    }
}