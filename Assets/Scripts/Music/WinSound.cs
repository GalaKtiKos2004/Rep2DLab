using System;
using UnityEngine;

/// <summary>
/// Проигрывает звук победы при завершении уровня.
/// </summary>
public class WinSound : MonoBehaviour
{
    /// <summary> Система воспроизведения звуков. </summary>
    [SerializeField] private GameSound _gameSound;

    /// <summary> Аудиофайл, содержащий звук победы. </summary>
    [SerializeField] private AudioClip _clip;

    /// <summary> Менеджер уровня, отвечающий за события победы. </summary>
    [SerializeField] private LevelManager _levelManager;

    /// <summary>
    /// Подписывается на событие победы при включении объекта.
    /// </summary>
    private void OnEnable()
    {
        _levelManager.Won += OnWon;
    }

    /// <summary>
    /// Отписывается от события победы при отключении объекта.
    /// </summary>
    private void OnDisable()
    {
        _levelManager.Won -= OnWon;
    }

    /// <summary>
    /// Вызывается при победе игрока и воспроизводит соответствующий звук.
    /// </summary>
    private void OnWon()
    {
        _gameSound.PlaySound(_clip);
    }
}
