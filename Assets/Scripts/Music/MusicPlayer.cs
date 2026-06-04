using UnityEngine;

/// <summary>
/// Обеспечивает непрерывное воспроизведение фоновой музыки между сценами.
/// Гарантирует, что объект с музыкой существует в единственном экземпляре.
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer _instance;

    /// <summary>
    /// Создает единственный экземпляр объекта, который не уничтожается при смене сцены.
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
