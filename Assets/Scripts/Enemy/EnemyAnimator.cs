using UnityEngine;

/// <summary>
/// Управляет анимациями врага, реагируя на события атаки и смерти.
/// </summary>
[RequireComponent(typeof(EnemyFighter))]
[RequireComponent(typeof(SkeletonMover))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private const string Attack = "Attack";
    private const string Death = "Death";
    private const string Move = "Speed";
    
    private Animator _animator;
    private EnemyFighter _enemyFighter;
    private SkeletonMover _skeletonMover;

    /// <summary>
    /// Инициализирует компоненты Animator и EnemyFighter.
    /// </summary>
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyFighter = GetComponent<EnemyFighter>();
        _skeletonMover = GetComponent<SkeletonMover>();
    }

    /// <summary>
    /// Подписывается на события атаки и смерти врага.
    /// </summary>
    private void OnEnable()
    {
        _enemyFighter.Attacked += OnAttack;
        _enemyFighter.Died += OnDied;
        _skeletonMover.Moved += OnMove;
    }

    /// <summary>
    /// Отписывается от событий атаки и смерти врага.
    /// </summary>
    private void OnDisable()
    {
        _enemyFighter.Attacked -= OnAttack;
        _enemyFighter.Died -= OnDied;
        _skeletonMover.Moved -= OnMove;
    }

    /// <summary>
    /// Вызывается при атаке врага. Запускает анимацию атаки.
    /// </summary>
    private void OnAttack()
    {
        _animator.SetTrigger(Attack);
    }

    /// <summary>
    /// Вызывается при смерти врага. Запускает анимацию смерти.
    /// </summary>
    private void OnDied()
    {
        _animator.SetTrigger(Death);
    }

    private void OnMove(float speed)
    {
        _animator.SetFloat(Move, speed);
    }
}
