using UnityEngine;

/// <summary>
/// Отвечает за воспроизведение звуков в игре.
/// </summary>
public class GameSound : MonoBehaviour
{
    /// <summary> Источник звука, используемый для воспроизведения аудио. </summary>
    [SerializeField] private AudioSource _audioSource;

    /// <summary>
    /// Проигрывает переданный аудиоклип.
    /// </summary>
    /// <param name="clip">Аудиофайл, который нужно воспроизвести.</param>
    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}
