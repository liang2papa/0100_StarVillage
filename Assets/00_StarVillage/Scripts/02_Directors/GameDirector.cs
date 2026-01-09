using System;
using UnityEngine;

// [Lv 2] 씬의 감독 (추상 클래스)
public abstract class GameDirector : MonoBehaviour
{
    public EGameState CurrentState { get; private set; }
    
    public event Action<EGameState> OnStateChanged;

    public virtual void InitClass(EGameState state)
    {
        CurrentState = state;
    }
    public virtual void StartGame()
    {
        CurrentState = EGameState.Playing;
        OnStateChanged.Invoke(CurrentState);
    }
    public virtual void PauseGame()
    {
        CurrentState = EGameState.Paused;
        OnStateChanged.Invoke(CurrentState);
    }
    public virtual void ResumeGame()
    {
        CurrentState = EGameState.Playing;
        OnStateChanged.Invoke(CurrentState);
    }
}
