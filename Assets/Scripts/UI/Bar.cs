using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Абстрактный класс, представляющий собой базовый индикатор (например, шкалу здоровья или маны).
/// Управляет значением шкалы через компонент Image.
/// </summary>
[RequireComponent(typeof(Image))]
public abstract class Bar : MonoBehaviour
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// Устанавливает значение шкалы.
    /// </summary>
    /// <param name="value">Новое значение шкалы (от 0 до 1).</param>
    protected void ChangeBarValue(float value)
    {
        _image.fillAmount = value;
    }

    /// <summary>
    /// Плавное уменьшение значения шкалы за заданное время.
    /// </summary>
    /// <param name="targetValue">Конечное значение шкалы (от 0 до 1).</param>
    /// <param name="smooothDecreaseDuration">Время, за которое шкала плавно уменьшится до целевого значения.</param>
    /// <returns>Корутина, выполняющая плавное уменьшение шкалы.</returns>
    protected virtual IEnumerator DecreaseBarSmoothly(float targetValue, float smooothDecreaseDuration)
    {
        float elapsedTime = 0f;
        float previousValue = _image.fillAmount;

        while (elapsedTime < smooothDecreaseDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedPosition = elapsedTime / smooothDecreaseDuration;
            float intermediateValue = Mathf.Lerp(previousValue, targetValue, normalizedPosition);
            _image.fillAmount = intermediateValue;

            yield return null;
        }
    }
}
