using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Показывает сюжетные диалоги при входе на уровень и перед переходом на следующий.
/// </summary>
public class StoryDirector : MonoBehaviour
{
    private static readonly string[] SkippedScenes = { "GameStart", "GameFinish" };

    public static StoryDirector Instance { get; private set; }

    private LevelStory[] _stories;
    private StoryDialogueUI _activeUi;
    private bool _isPlaying;

    public bool IsPlaying => _isPlaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        _stories = Resources.LoadAll<LevelStory>("Story");
        SceneManager.sceneLoaded += OnSceneLoaded;
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

        LevelStory story = FindStory(scene.name);

        if (story == null || story.IntroLines.Count == 0)
        {
            return;
        }

        StartCoroutine(PlayIntroWhenPlayerReady(story.IntroLines));
    }

    public void PlayOutro(string sceneName, Action onComplete)
    {
        LevelStory story = FindStory(sceneName);

        if (story == null || story.OutroLines.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(PlayLines(story.OutroLines, onComplete));
    }

    private IEnumerator PlayIntroWhenPlayerReady(IReadOnlyList<string> lines)
    {
        while (FindObjectOfType<PlayerMover>() == null)
        {
            yield return null;
        }

        yield return StartCoroutine(PlayLines(lines, null));
    }

    private IEnumerator PlayLines(IReadOnlyList<string> lines, Action onComplete)
    {
        while (_isPlaying)
        {
            yield return null;
        }

        _isPlaying = true;
        PlayerFreeze.SetFrozen(true);
        QuestTrackerUI.SetTrackerVisible(false);

        if (_activeUi != null)
        {
            Destroy(_activeUi.gameObject);
        }

        bool finished = false;
        _activeUi = StoryDialogueUI.Create();
        _activeUi.Play(lines, () => finished = true, 6f, 3f);

        yield return new WaitUntil(() => finished);

        _activeUi = null;
        PlayerFreeze.SetFrozen(false);
        _isPlaying = false;
        QuestTrackerUI.SetTrackerVisible(true);
        onComplete?.Invoke();
    }

    private LevelStory FindStory(string sceneName)
    {
        if (_stories == null)
        {
            return null;
        }

        foreach (var story in _stories)
        {
            if (story != null && story.SceneName == sceneName)
            {
                return story;
            }
        }

        return null;
    }

    private static bool ShouldSkip(string sceneName)
    {
        foreach (var skipped in SkippedScenes)
        {
            if (skipped == sceneName)
            {
                return true;
            }
        }

        return false;
    }

}
