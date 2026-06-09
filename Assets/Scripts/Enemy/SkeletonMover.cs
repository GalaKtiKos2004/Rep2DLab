using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SkeletonMover : MonoBehaviour
{
    [SerializeField] private EnemyFighter _fighter;

    private Rigidbody2D _rb;

    public event Action<float> Moved;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.mass = 1000f;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnEnable()
    {
        _fighter.Died += OnDied;
    }

    private void OnDisable()
    {
        _fighter.Died -= OnDied;
    }

    private void Update()
    {
        Moved?.Invoke(0f);
    }

    private void FixedUpdate()
    {
        _rb.velocity = Vector2.zero;
    }

    private void OnDied()
    {
        _rb.velocity = Vector2.zero;
    }
}
