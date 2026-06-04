using UnityEngine;

[RequireComponent(typeof(NPCMover))]
[RequireComponent(typeof(Animator))]
public class NPCAnimator : MonoBehaviour
{
    private const string Idle = "Idle";
    
    private NPCMover _npcMover;
    private Animator _animator;

    private void Awake()
    {
        _npcMover = GetComponent<NPCMover>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _npcMover.TriggerEntered += OnTriggeredEntered;
    }

    private void OnDisable()
    {
        _npcMover.TriggerEntered -= OnTriggeredEntered;
    }

    private void OnTriggeredEntered()
    {
        _animator.SetTrigger(Idle);
    }
}
