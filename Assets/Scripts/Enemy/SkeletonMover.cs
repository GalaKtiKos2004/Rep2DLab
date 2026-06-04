using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SkeletonMover : MonoBehaviour
{
    [Header("Параметры движения")]
    [SerializeField] private Transform _target;           // Цель (например, игрок)
    [SerializeField] private float _maxSpeed = 3f;          // Заданная скорость движения
    [SerializeField] private float _detectionRange = 5f;    // Радиус обнаружения цели
    [SerializeField] private EnemyFighter _fighter;

    private Rigidbody2D _rb;
    private bool _isDead = false;

    public event Action<float> Moved;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Задаём высокую массу, чтобы внешние импульсы были несущественны,
        // но помните, что движение будем задавать напрямую
        _rb.mass = 1000f;
        
        // Замораживаем вращение, чтобы столкновения не вызывали поворотов
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnEnable()
    {
        _fighter.Died += OnDied;
        _fighter.CanAttack += OnCanAttack;
    }

    private void OnDisable()
    {
        _fighter.Died -= OnDied;
        _fighter.CanAttack -= OnCanAttack;
    }

    private void Update()
    {
        Moved.Invoke(_rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (_target == null)
            return;

        float distance = Vector2.Distance(_rb.position, _target.position);
        if (distance > _detectionRange)
        {
            // Если цель далеко, враг стоит на месте
            _rb.velocity = Vector2.zero;
            return;
        }

        // Вычисляем направление к цели
        Vector2 direction = (((Vector2)_target.position) - _rb.position).normalized;
        
        // Задаём скорость напрямую — это «перезапишет» любые физические импульсы от столкновений
        _rb.velocity = direction * _maxSpeed;
    }

    private void OnDied()
    {
        _rb.velocity = Vector2.zero;
        _maxSpeed = 0f;
        _isDead = true;
    }

    private void OnCanAttack(bool canAttack)
    {
        if (_isDead)
        {
            return;
        }

        if (canAttack == false)
        {
            _maxSpeed = 1f;
        }
        else
        {
            _maxSpeed = 0f;
        }
    }
}
