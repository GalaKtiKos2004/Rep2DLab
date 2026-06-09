using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Временный UI для показа сюжетных реплик между уровнями.
/// </summary>
public class StoryDialogueUI : MonoBehaviour
{
    private StartDialogue _dialogue;
    private Action _onFinished;

    public static StoryDialogueUI Create()
    {
        var root = new GameObject("StoryDialogueUI");
        var ui = root.AddComponent<StoryDialogueUI>();
        ui.Build();
        return ui;
    }

    public void Play(IReadOnlyList<string> lines, Action onFinished, float textDuration = 4f, float wait = 2f)
    {
        if (lines == null || lines.Count == 0)
        {
            onFinished?.Invoke();
            Destroy(gameObject);
            return;
        }

        _onFinished = onFinished;
        _dialogue.Configure(textDuration, wait);
        _dialogue.SetDialogues(lines);
        _dialogue.DialogueFinished += OnDialogueFinished;
        _dialogue.StartText();
    }

    private void OnDialogueFinished()
    {
        _dialogue.DialogueFinished -= OnDialogueFinished;
        _onFinished?.Invoke();
        Destroy(gameObject);
    }

    private void Build()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        var panel = CreateChild("Panel", transform);
        var panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.85f);
        Stretch(panel, 0, 0, 0, 0);

        var textRect = CreateChild("Text", panel);
        Stretch(textRect, 80, 80, 80, 80);

        var text = textRect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        _dialogue = textRect.gameObject.AddComponent<StartDialogue>();
        _dialogue.Configure(4f, 2f);
    }

    private static RectTransform CreateChild(string name, Transform parent)
    {
        var child = new GameObject(name, typeof(RectTransform));
        child.transform.SetParent(parent, false);
        return child.GetComponent<RectTransform>();
    }

    private static void Stretch(RectTransform rect, float left, float right, float top, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }
}
