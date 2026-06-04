using UnityEngine;

/// <summary>
/// Компонент, предотвращающий вращение канваса, например, при вращении камеры.
/// Полезен для UI-элементов, которые должны всегда оставаться в одной ориентации.
/// </summary>
public class CanvasRotator : MonoBehaviour
{
    private void LateUpdate()
    {
        // Сбрасывает поворот объекта, чтобы он всегда оставался неизменным
        transform.rotation = Quaternion.identity;
    }
}
