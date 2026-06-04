using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет загрузкой сцен и сохранением инвентаря между уровнями.
/// Реализует паттерн Singleton.
/// </summary>
public class SceneController : MonoBehaviour
{
    /// <summary> Единственный экземпляр SceneController. </summary>
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Загружает указанную сцену и сохраняет инвентарь перед её загрузкой.
    /// </summary>
    /// <param name="sceneName">Название сцены для загрузки.</param>
    /// <param name="currentInventory">Текущий инвентарь игрока, который нужно сохранить.</param>
    public void LoadScene(string sceneName, Inventory currentInventory, EqupimentManager equipment)
    {
        GameSession.Instance.SaveInventory(currentInventory, equipment);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Загружает инвентарь после загрузки новой сцены.
    /// </summary>
    /// <param name="scene">Загруженная сцена.</param>
    /// <param name="mode">Режим загрузки сцены.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Inventory newInventory = FindObjectOfType<Inventory>();
        EqupimentManager equipment = FindObjectOfType<EqupimentManager>();

        if (newInventory != null)
        {
            GameSession.Instance.LoadInventory(newInventory, equipment);
        }
    }
}
