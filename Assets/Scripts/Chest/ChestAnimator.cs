using UnityEngine;

/// <summary>
/// Компонент, управляющий анимацией сундука.
/// Ожидает компонент Chest и запускает анимацию открытия при взаимодействии с сундуком.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Chest))]
public class ChestAnimator : MonoBehaviour
{
    private const string Open = "Open"; // Название триггера анимации открытия
    
    private Chest _chest;
    private Animator _animator;

    /// <summary>
    /// Инициализация компонентов.
    /// </summary>
    private void Awake()
    {
        _chest = GetComponent<Chest>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Подписка на событие открытия сундука при активации объекта.
    /// </summary>
    private void OnEnable()
    {
        _chest.Opening += OnOpening;
    }

    /// <summary>
    /// Отписка от события открытия сундука при деактивации объекта.
    /// </summary>
    private void OnDisable()
    {
        _chest.Opening -= OnOpening;
    }

    /// <summary>
    /// Запускает анимацию открытия сундука.
    /// </summary>
    private void OnOpening()
    {
        _animator.SetTrigger(Open);
    }
}
