using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneLoader : MonoBehaviour
{
    [SerializeField] string _sceneName;
    
    public void SceneLoad()
    {
        Debug.Log("start");
        SceneManager.LoadScene(_sceneName);
    }
}
