using UnityEngine;
using System;

/// <summary>
/// Класс, представляющий систему здоровья персонажа.
/// Позволяет получать урон, отслеживать изменения здоровья и вызывать событие смерти.
/// </summary>
public class Health
{
    private float _currentHealth;
    private float _maxHealth;

    /// <summary>
    /// Событие вызывается при смерти (когда здоровье достигает 0).
    /// </summary>
    public event Action Died;

    /// <summary>
    /// Событие вызывается при изменении здоровья.
    /// </summary>
    /// <param name="currentHealth">Текущее значение здоровья.</param>
    /// <param name="maxHealth">Максимальное значение здоровья.</param>
    public event Action<float, float> Changed;

    /// <summary>
    /// Создает новый объект здоровья.
    /// </summary>
    /// <param name="health">Начальное значение здоровья (используется как максимальное).</param>
    public Health(float health)
    {
        _currentHealth = health;
        _maxHealth = health;
    }

    /// <summary>
    /// Применяет урон к здоровью.
    /// </summary>
    /// <param name="damage">Количество урона.</param>
    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
        Changed?.Invoke(_currentHealth, _maxHealth);
        
        if (_currentHealth <= 0)
        {
            Died?.Invoke();
        }
    }
}
