using System;
using UnityEngine;

// [Lv 2] 씬의 감독 (추상 클래스)
// 게임 상태를 가지고 있으며 이를 알리는 역할을 함
// 필드 관리자 또는 마을 관리자가 상속받아 사용
public abstract class GameDirector : MonoBehaviour
{
    [field: SerializeField] public EGameState CurrentState { get; private set; }
    
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
