using System;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum State
    {
        Idle, Roll, Open, Reduce
    }
    State state;
    public State CurState
    {
        get => state;
        set
        {
            if (state == value) return;
            animator.SetInteger("State", (int)value);
            state = value;
        }
    }

    [SerializeField] Animator animator;

    public event Action EndRollEvent, OnOpenEvent;

    void EndRoll() => EndRollEvent?.Invoke();
    void Open() => OnOpenEvent?.Invoke();
}
