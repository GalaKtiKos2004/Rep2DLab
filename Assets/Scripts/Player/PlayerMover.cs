using System;
using UnityEngine;

/// <summary>
/// Отвечает за передвижение игрока, обработку ввода и физику.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;         // Скорость передвижения игрока
    [SerializeField] private PlayerFighter _playerFighter;    // Компонент боевой системы игрока

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private bool _isDied = false;

    // Стандартные масштабы для поворота спрайта
    private readonly Vector2 _defaultScale = new Vector2(1, 1);
    private readonly Vector2 _flippedScale = new Vector2(-1, 1);

    public event Action<float> Moved;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerFighter = GetComponent<PlayerFighter>();

        _rigidbody.gravityScale = 0f;
        // Изначально блокируем вращение
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidbody.isKinematic = false;
    }

    private void OnEnable()
    {
        _playerFighter.Died += OnDied;
    }

    private void OnDisable()
    {
        _playerFighter.Died -= OnDied;
    }

    /// <summary>
    /// Останавливает движение игрока после его смерти.
    /// </summary>
    private void OnDied()
    {
        _isDied = true;
        _rigidbody.velocity = Vector2.zero;
        _moveSpeed = 0f;
        // Полностью блокируем движение
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void Update()
    {
        if (_isDied)
            return;

        // Получаем ввод
        _movementInput.x = Input.GetAxisRaw("Horizontal");
        _movementInput.y = Input.GetAxisRaw("Vertical");
        _movementInput = _movementInput.normalized;

        Moved?.Invoke(Mathf.Max(Mathf.Abs(_movementInput.x), Mathf.Abs(_movementInput.y)));

        // Поворот спрайта по горизонтали
        if (_movementInput.x > 0)
            transform.localScale = _defaultScale;
        else if (_movementInput.x < 0)
            transform.localScale = _flippedScale;
    }

    private void FixedUpdate()
    {
        if (_isDied)
            return;

        if (_movementInput == Vector2.zero)
        {
            // Если игрок не двигается, блокируем позицию, чтобы его не сдвигали столкновения
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            _rigidbody.velocity = Vector2.zero;
        }
        else
        {
            // Если ввод есть, снимаем блокировку по осям (оставляем только заморозку вращения)
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidbody.velocity = _movementInput * _moveSpeed;
        }
    }

    public void SetSpeed(float multiplier)
    {
        _moveSpeed *= multiplier;
    }
}
