using UnityEngine;

/// <summary>
/// Управляет анимациями игрока, реагируя на его передвижение, атаки, смерть и победу.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerFighter))]
public class PlayerAnimator : MonoBehaviour
{
    private const string Speed = "Speed";
    private const string Attack = "Attack";
    private const string Death = "Death";
    private const string Win = "Win";

    /// <summary> Менеджер уровня, используется для отслеживания победы. </summary>
    [SerializeField] private LevelManager _levelManager;
    
    private Animator _animator;
    private PlayerMover _playerMover;
    private PlayerFighter _playerFighter;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMover = GetComponent<PlayerMover>();
        _playerFighter = GetComponent<PlayerFighter>();
    }

    private void OnEnable()
    {
        _playerMover.Moved += OnMoved;
        _playerFighter.Attacked += OnAttack;
        _playerFighter.Died += OnDied;
        _levelManager.Won += OnWon;
    }

    private void OnDisable()
    {
        _playerMover.Moved -= OnMoved;
        _playerFighter.Attacked -= OnAttack;
        _playerFighter.Died -= OnDied;
        _levelManager.Won -= OnWon;
    }

    /// <summary>
    /// Вызывается при победе игрока и запускает анимацию победы.
    /// </summary>
    private void OnWon()
    {
        _animator.SetTrigger(Win);
    }

    /// <summary>
    /// Вызывается при движении игрока и обновляет скорость анимации.
    /// </summary>
    /// <param name="speed">Текущая скорость игрока.</param>
    private void OnMoved(float speed)
    {
        _animator.SetFloat(Speed, speed);
    }

    /// <summary>
    /// Вызывается при атаке игрока и запускает анимацию атаки.
    /// </summary>
    private void OnAttack()
    {
        _animator.SetTrigger(Attack);
    }

    /// <summary>
    /// Вызывается при смерти игрока и запускает анимацию смерти.
    /// </summary>
    private void OnDied()
    {
        _animator.SetTrigger(Death);
    }
}
