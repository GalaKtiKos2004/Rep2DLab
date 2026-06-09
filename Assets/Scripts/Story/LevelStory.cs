using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelStory", menuName = "Story/Level Story")]
public class LevelStory : ScriptableObject
{
    public string SceneName;
    [TextArea(2, 6)] public List<string> IntroLines = new List<string>();
    [TextArea(2, 6)] public List<string> OutroLines = new List<string>();
}
