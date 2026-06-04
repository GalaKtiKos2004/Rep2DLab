using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyFighter : Fighter
{
    /// <summary>
    /// Аниматор для управления анимацией врага.
    /// </summary>
    [SerializeField] private Animator _animator;
    
    /// <summary>
    /// Время, за которое враг исчезает после смерти.
    /// </summary>
    [SerializeField] private float _dyingTime;
    
    private SpriteRenderer _spriteRenderer;
    private bool _animationEnding = true;
    private bool _isDead = false;
    
    /// <summary>
    /// Событие, вызываемое при атаке врага.
    /// </summary>
    public event Action Attacked;

    public event Action<bool> CanAttack;
    
    /// <summary>
    /// Событие, вызываемое при смерти врага.
    /// </summary>
    public event Action Died;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (InTrigger == false)
        {
            CanAttack?.Invoke(false);
            return;
        }
        
        CanAttack?.Invoke(true);
        
        // Если анимация атаки не завершена, враг мертв, есть кулдаун или игрок не в зоне триггера, не атакуем
        if (_animationEnding == false || _isDead || IsCooldown)
        {
            return;
        }

        // Если в зону триггера вошел игрок, инициируем атаку
        if (CollidedObject.TryGetComponent(out PlayerFighter _))
        {
            Attacked?.Invoke();
            _animationEnding = false;
        }
    }
    
    /// <summary>
    /// Метод, вызываемый при смерти врага.
    /// </summary>
    protected override void OnDied()
    {
        Died?.Invoke();
        _isDead = true;
        StartCoroutine(Dying());
    }

    /// <summary>
    /// Выполняет атаку, если враг столкнулся с игроком и находится в зоне триггера.
    /// </summary>
    private void PreformAttack()
    {
        if (CollidedObject.TryGetComponent(out PlayerFighter playerFighter) && InTrigger)
        {
            Attack(playerFighter);
        }
    }

    /// <summary>
    /// Устанавливает флаг окончания анимации атаки.
    /// </summary>
    private void AnimationEnded()
    {
        _animationEnding = true;
    }

    /// <summary>
    /// Корутина, выполняющая плавное исчезновение врага после смерти.
    /// </summary>
    private IEnumerator Dying()
    {
        float elapsedTime = 0f;
        Color color = _spriteRenderer.color;

        while (elapsedTime < _dyingTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / _dyingTime);
            _spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}