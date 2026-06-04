using System;
using UnityEngine;

/// <summary>
/// Класс, представляющий триггерную зону для обнаружения входа и выхода объектов.
/// </summary>
public class Trigger : MonoBehaviour
{
    /// <summary>
    /// Событие вызывается, когда объект входит в триггерную зону.
    /// </summary>
    public event Action<Collider2D> TriggerEntered;

    /// <summary>
    /// Событие вызывается, когда объект выходит из триггерной зоны.
    /// </summary>
    public event Action<Collider2D> TriggerExited;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEntered?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TriggerExited?.Invoke(other);
    }
}
