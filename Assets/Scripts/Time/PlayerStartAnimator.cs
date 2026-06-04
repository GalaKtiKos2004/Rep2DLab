using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerStartAnimator : MonoBehaviour
{
    private const string Move = "Move";
    
    [SerializeField] private StartDialogue _dialogues;
    
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
