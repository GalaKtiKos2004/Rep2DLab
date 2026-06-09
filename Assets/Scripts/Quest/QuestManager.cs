using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    private static readonly string[] SkippedScenes = { "GameStart", "GameFinish" };

    public static QuestManager Instance { get; private set; }

    public event Action QuestsChanged;

    private readonly Dictionary<string, QuestProgressData> _progress = new Dictionary<string, QuestProgressData>();
    private Quest[] _quests = Array.Empty<Quest>();
    private bool _isShowingDialogue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        _quests = Resources.LoadAll<Quest>("Quests");
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (FindObjectOfType<QuestTrackerUI>() == null)
        {
            QuestTrackerUI.Create();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ShouldSkip(scene.name))
        {
            return;
        }

        StartCoroutine(AutoStartQuestsWhenReady(scene.name));
    }

    private IEnumerator AutoStartQuestsWhenReady(string sceneName)
    {
        yield return WaitForStoryDialogue();

        foreach (Quest quest in _quests)
        {
            if (quest == null || quest.AutoStartOnSceneEnter == false)
            {
                continue;
            }

            if (quest.SceneName != sceneName)
            {
                continue;
            }

            TryAcceptQuest(quest);
        }
    }

    public List<QuestProgressData> GetSaveData()
    {
        return new List<QuestProgressData>(_progress.Values);
    }

    public void LoadSaveData(List<QuestProgressData> data)
    {
        _progress.Clear();

        if (data == null)
        {
            QuestsChanged?.Invoke();
            return;
        }

        foreach (QuestProgressData entry in data)
        {
            if (entry == null || string.IsNullOrEmpty(entry.QuestId))
            {
                continue;
            }

            _progress[entry.QuestId] = entry;
        }

        QuestsChanged?.Invoke();
    }

    public bool TryAcceptQuest(Quest quest)
    {
        if (quest == null || string.IsNullOrEmpty(quest.Id))
        {
            return false;
        }

        QuestProgressData progress = GetOrCreateProgress(quest.Id);

        if (progress.State != QuestState.NotStarted)
        {
            return false;
        }

        progress.State = QuestState.Active;
        progress.CurrentCount = 0;
        QuestsChanged?.Invoke();
        return true;
    }

    public bool TryAcceptQuestWithDialogue(Quest quest)
    {
        if (TryAcceptQuest(quest) == false)
        {
            return false;
        }

        if (quest.AcceptLines.Count > 0)
        {
            StartCoroutine(ShowLines(quest.AcceptLines));
        }

        return true;
    }

    public void NotifyEnemyKilled()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        foreach (Quest quest in _quests)
        {
            if (quest == null || quest.Type != QuestType.KillEnemies)
            {
                continue;
            }

            if (quest.SceneName != sceneName)
            {
                continue;
            }

            QuestProgressData progress = GetOrCreateProgress(quest.Id);

            if (progress.State != QuestState.Active)
            {
                continue;
            }

            progress.CurrentCount++;

            if (progress.CurrentCount >= quest.TargetCount)
            {
                CompleteQuest(quest, progress);
            }
            else
            {
                QuestsChanged?.Invoke();
            }
        }
    }

    public IReadOnlyList<Quest> GetActiveQuests()
    {
        List<Quest> active = new List<Quest>();

        foreach (Quest quest in _quests)
        {
            if (quest == null)
            {
                continue;
            }

            if (GetOrCreateProgress(quest.Id).State == QuestState.Active)
            {
                active.Add(quest);
            }
        }

        return active;
    }

    public QuestProgressData GetProgress(string questId)
    {
        return GetOrCreateProgress(questId);
    }

    public bool IsQuestCompleted(string questId)
    {
        if (string.IsNullOrEmpty(questId))
        {
            return true;
        }

        return GetOrCreateProgress(questId).State == QuestState.Completed;
    }

    public Quest FindQuest(string questId)
    {
        foreach (Quest quest in _quests)
        {
            if (quest != null && quest.Id == questId)
            {
                return quest;
            }
        }

        return null;
    }

    private void CompleteQuest(Quest quest, QuestProgressData progress)
    {
        progress.State = QuestState.Completed;
        GiveReward(quest);
        QuestsChanged?.Invoke();
        Debug.Log($"Квест выполнен: {quest.Title}. Награда выдана.");
    }

    private void GiveReward(Quest quest)
    {
        if (quest.RewardItem == null)
        {
            return;
        }

        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null && inventory.AddItem(quest.RewardItem))
        {
            Debug.Log($"Награда за квест: {quest.RewardItem.Name}");
        }
    }

    private QuestProgressData GetOrCreateProgress(string questId)
    {
        if (_progress.TryGetValue(questId, out QuestProgressData progress) == false)
        {
            progress = new QuestProgressData
            {
                QuestId = questId,
                State = QuestState.NotStarted,
                CurrentCount = 0
            };
            _progress[questId] = progress;
        }

        return progress;
    }

    private IEnumerator ShowLines(IReadOnlyList<string> lines)
    {
        yield return WaitForStoryDialogue();

        while (_isShowingDialogue)
        {
            yield return null;
        }

        _isShowingDialogue = true;
        PlayerFreeze.SetFrozen(true);
        QuestTrackerUI.SetTrackerVisible(false);

        bool finished = false;
        StoryDialogueUI ui = StoryDialogueUI.Create();
        ui.Play(lines, () => finished = true, 6f, 3f);
        yield return new WaitUntil(() => finished);

        PlayerFreeze.SetFrozen(false);
        _isShowingDialogue = false;
        QuestTrackerUI.SetTrackerVisible(true);
    }

    private static IEnumerator WaitForStoryDialogue()
    {
        while (StoryDirector.Instance != null && StoryDirector.Instance.IsPlaying)
        {
            yield return null;
        }
    }

    private static bool ShouldSkip(string sceneName)
    {
        foreach (string skipped in SkippedScenes)
        {
            if (skipped == sceneName)
            {
                return true;
            }
        }

        return false;
    }

}
