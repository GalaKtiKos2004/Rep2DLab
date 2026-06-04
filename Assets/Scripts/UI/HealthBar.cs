using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Компонент, управляющий шкалой здоровья.
/// Реализует плавное уменьшение при получении урона и удаление при смерти.
/// </summary>
public class HealthBar : Bar
{
    /// <summary> Изображение, используемое для отображения шкалы здоровья. </summary>
    [SerializeField] private Image _bar;

    /// <summary> Время, за которое шкала плавно уменьшается при получении урона. </summary>
    [SerializeField] private float _smooothDecreaseDuration = 0.25f;

    private Health _health;

    private void OnDisable()
    {
        _health.Changed -= UpdateBar;
        _health.Died -= DeleteBar;
    }

    /// <summary>
    /// Инициализирует шкалу здоровья, подписываясь на изменения здоровья.
    /// </summary>
    /// <param name="health">Объект здоровья, к которому привязана шкала.</param>
    public void Init(Health health)
    {
        _health = health;

        _health.Changed += UpdateBar;
        _health.Died += DeleteBar;
    }

    /// <summary>
    /// Обновляет значение шкалы здоровья.
    /// </summary>
    /// <param name="value">Текущее здоровье.</param>
    /// <param name="maxValue">Максимальное здоровье.</param>
    private void UpdateBar(float value, float maxValue)
    {
        StartCoroutine(DecreaseBarSmoothly(value / maxValue, _smooothDecreaseDuration));
    }

    /// <summary>
    /// Удаляет шкалу здоровья при смерти персонажа.
    /// </summary>
    private void DeleteBar()
    {
        Destroy(gameObject);
        Destroy(_bar.gameObject);
    }
}
