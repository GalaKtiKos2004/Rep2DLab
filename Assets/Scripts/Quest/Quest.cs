using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public string Id;
    public string Title;
    [TextArea(2, 4)] public string Description;

    public QuestType Type = QuestType.KillEnemies;
    public string SceneName;
    public int TargetCount = 3;
    public bool AutoStartOnSceneEnter = true;

    public Item RewardItem;

    [TextArea(2, 4)] public List<string> AcceptLines = new List<string>();
    [TextArea(2, 4)] public List<string> CompleteLines = new List<string>();
}
