using UnityEngine;

/// <summary>
/// Инициализирует персонажа, создавая его здоровье и связывая с бойцом и полоской здоровья.
/// </summary>
public class CharacterBootstraper : MonoBehaviour
{
    /// <summary>
    /// Начальное количество здоровья персонажа.
    /// </summary>
    [SerializeField] private float _health;

    /// <summary>
    /// Ссылка на компонент бойца, который управляет атакой и получением урона.
    /// </summary>
    [SerializeField] private Fighter _fighter;

    /// <summary>
    /// Ссылка на компонент полоски здоровья, который визуализирует текущее состояние здоровья.
    /// </summary>
    [SerializeField] private HealthBar _healthBar;

    /// <summary>
    /// Вызывается при инициализации объекта. Создает объект здоровья и передает его бойцу и полоске здоровья.
    /// </summary>
    private void Awake()
    {
        Health health = new(_health);
        _fighter.Init(health);
        _healthBar.Init(health);
    }
}
