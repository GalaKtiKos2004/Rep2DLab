using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FinishAnimator : MonoBehaviour
{
    private const string Move = "Move";
    
    [SerializeField] private FinishDialog _dialogues;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _dialogues.DialogueFinished += OnMove;
    }

    private void OnDisable()
    {
        _dialogues.DialogueFinished -= OnMove;
    }

    private void OnMove()
    {
        _animator.SetTrigger(Move);
    }
}
