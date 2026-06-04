using System.Collections;
using UnityEngine;

/// <summary>
/// Абстрактный класс бойца, определяющий базовую механику атаки, получения урона и перезарядки.
/// </summary>
public abstract class Fighter : MonoBehaviour
{
    /// <summary>
    /// Базовый урон бойца.
    /// </summary>
    [SerializeField] private float _damage;

    /// <summary>
    /// Время перезарядки атаки.
    /// </summary>
    [SerializeField] private float _cooldownTime;

    /// <summary>
    /// Базовая защита бойца.
    /// </summary>
    [SerializeField] private float _baseDefense;

    /// <summary>
    /// Ссылка на триггер для определения, находится ли противник в зоне атаки.
    /// </summary>
    [SerializeField] private Trigger _trigger;

    private WaitForSeconds _cooldown;
    private Health _health;

    /// <summary>
    /// Ссылка на компонент здоровья бойца.
    /// </summary>
    protected Health Health => _health;

    /// <summary>
    /// Показывает, находится ли противник в зоне атаки.
    /// </summary>
    protected bool InTrigger { get; private set; } = false;

    /// <summary>
    /// Ссылка на объект, с которым произошло столкновение.
    /// </summary>
    protected Collider2D CollidedObject { get; private set; }

    /// <summary>
    /// Показывает, находится ли боец в состоянии перезарядки атаки.
    /// </summary>
    protected bool IsCooldown { get; private set; } = false;

    /// <summary>
    /// Показывает, погиб ли боец.
    /// </summary>
    public bool IsDead { get; protected set; } = false;

    private void OnEnable()
    {
        _cooldown = new WaitForSeconds(_cooldownTime);
        _trigger.TriggerEntered += OnTriggerEntered;
        _trigger.TriggerExited += OnTriggerExited;
    }

    private void OnDisable()
    {
        _trigger.TriggerEntered -= OnTriggerEntered;
        _trigger.TriggerExited -= OnTriggerExited;
        _health.Died -= OnDied;
    }

    /// <summary>
    /// Инициализирует бойца, присваивая ему компонент здоровья.
    /// </summary>
    /// <param name="health">Компонент здоровья.</param>
    public void Init(Health health)
    {
        _health = health;
        _health.Died += OnDied;
    }

    /// <summary>
    /// Метод вызывается при смерти бойца.
    /// </summary>
    protected virtual void OnDied()
    {
        IsDead = true;
    }

    /// <summary>
    /// Выполняет атаку по другому бойцу.
    /// </summary>
    /// <param name="fighter">Цель атаки.</param>
    protected void Attack(Fighter fighter)
    {
        if (IsCooldown == false && fighter.IsDead == false)
        {
            float totalDamage = GetTotalDamage();
            fighter.TakeDamage(totalDamage);
            StartCoroutine(Cooldown());
        }
    }

    /// <summary>
    /// Получает урон с учетом защиты.
    /// </summary>
    /// <param name="damage">Полученный урон.</param>
    private void TakeDamage(float damage)
    {
        float totalDefense = GetTotalDefense();
        float finalDamage = Mathf.Max(damage - totalDefense, 0);
        _health.TakeDamage(finalDamage);
    }

    /// <summary>
    /// Возвращает итоговый урон бойца. Может быть переопределен в подклассах.
    /// </summary>
    /// <returns>Общий урон.</returns>
    protected virtual float GetTotalDamage()
    {
        return _damage;
    }

    /// <summary>
    /// Возвращает итоговую защиту бойца. Может быть переопределен в подклассах.
    /// </summary>
    /// <returns>Общая защита.</returns>
    protected virtual float GetTotalDefense()
    {
        return _baseDefense;
    }

    /// <summary>
    /// Обрабатывает вход в триггерную зону.
    /// </summary>
    /// <param name="other">Объект, вошедший в зону.</param>
    private void OnTriggerEntered(Collider2D other)
    {
        if (other.TryGetComponent(out Fighter _) == false)
        {
            return;
        }

        InTrigger = true;
        CollidedObject = other;
    }

    /// <summary>
    /// Обрабатывает выход из триггерной зоны.
    /// </summary>
    /// <param name="other">Объект, покинувший зону.</param>
    private void OnTriggerExited(Collider2D other)
    {
        if (other.TryGetComponent(out Fighter _) == false)
        {
            return;
        }

        InTrigger = false;
    }

    /// <summary>
    /// Запускает перезарядку атаки.
    /// </summary>
    private IEnumerator Cooldown()
    {
        IsCooldown = true;
        yield return _cooldown;
        IsCooldown = false;
    }
}
