using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управляет уровнем, отслеживает уничтожение всех врагов и переход на следующий уровень.
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary> Список врагов на уровне. </summary>
    [SerializeField] private List<EnemyFighter> _enemys;

    /// <summary> Инвентарь игрока. </summary>
    [SerializeField] private Inventory _inventory;
    
    /// <summary> Экипировка игрока. </summary>
    [SerializeField] private EqupimentManager _equipment;

    /// <summary> Контроллер смены сцен. </summary>
    [SerializeField] private SceneController _sceneController;

    /// <summary> Задержка перед переходом на новый уровень. </summary>
    [SerializeField] private float _delay = 3f;

    /// <summary> Имя сцены, которая загрузится после победы. </summary>
    [SerializeField] private string _newSceneName;

    /// <summary> Объект, управляющий задержкой перед сменой сцены. </summary>
    private WaitForSeconds _wait;

    /// <summary> Количество уничтоженных врагов. </summary>
    private int _diedCount = 0;

    /// <summary> Событие, вызываемое при победе на уровне. </summary>
    public event Action Won;

    private void OnEnable()
    {
        _wait = new WaitForSeconds(_delay);

        foreach (var enemy in _enemys)
        {
            enemy.Died += OnDied;
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in _enemys)
        {
            enemy.Died -= OnDied;
        }
    }

    /// <summary>
    /// Вызывается при уничтожении врага. Проверяет, все ли враги побеждены.
    /// </summary>
    private void OnDied()
    {
        _diedCount++;

        if (_diedCount == _enemys.Count)
        {
            Won?.Invoke();
            StartCoroutine(NewLevelDelay());
        }
    }

    /// <summary>
    /// Запускает задержку перед переходом на новый уровень.
    /// </summary>
    private IEnumerator NewLevelDelay()
    {
        yield return _wait;

        _sceneController.LoadScene(_newSceneName, _inventory, _equipment);
    }
}
