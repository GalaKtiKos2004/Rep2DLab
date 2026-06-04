using UnityEngine;

/// <summary>
/// Класс, отвечающий за поворот скелета в сторону цели.
/// Автоматически поворачивает объект в сторону _target.
/// </summary>
public class SkeletonRotator : MonoBehaviour
{
    /// <summary>
    /// Цель, на которую должен смотреть скелет.
    /// </summary>
    [SerializeField] private Transform _target;

    /// <summary>
    /// Скелет, которого надо повернуть
    /// </summary>
    [SerializeField] private Transform _skeleton;

    /// <summary>
    /// Ссылка на компонент файтера игрока.
    /// </summary>
    [SerializeField] private EnemyFighter _fighter;


    private readonly Quaternion _leftRotation = Quaternion.Euler(0, 180, 0);
    private readonly Quaternion _rightRotation = Quaternion.Euler(0, 0, 0);
    private bool _canRotate = true;
    private bool _inTrigger = false;

    private void OnEnable()
    {
        _fighter.Died += OnDied;
    }

    private void OnDisable()
    {
        _fighter.Died -= OnDied;
    }

    /// <summary>
    /// Проверяет позицию цели и поворачивает скелет в её сторону.
    /// </summary>
    private void Update()
    {
        if (_canRotate == false || _inTrigger == false || _target == null)
        {
            return;
        }

        if (_target.position.x > transform.position.x)
        {
            _skeleton.rotation = _rightRotation;
        }
        else
        {
            _skeleton.rotation = _leftRotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == _target)
        {
            _inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == _target)
        {
            _inTrigger = false;
        }
    }

    /// <summary>
    /// Останавливает поворот скелета при его смерти.
    /// </summary>
    private void OnDied()
    {
        _canRotate = false;
    }
}
