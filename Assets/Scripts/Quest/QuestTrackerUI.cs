using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Показывает активные квесты вверху по центру экрана.
/// </summary>
public class QuestTrackerUI : MonoBehaviour
{
    private static QuestTrackerUI _instance;

    private GameObject _panel;
    private Text _text;

    public static QuestTrackerUI Create()
    {
        var root = new GameObject("QuestTrackerUI");
        var tracker = root.AddComponent<QuestTrackerUI>();
        tracker.Build();
        DontDestroyOnLoad(root);
        _instance = tracker;
        return tracker;
    }

    public static void SetTrackerVisible(bool visible)
    {
        if (_instance != null && _instance._panel != null)
        {
            _instance._panel.SetActive(visible);
        }
    }

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.QuestsChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.QuestsChanged -= Refresh;
        }
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (_text == null || QuestManager.Instance == null)
        {
            return;
        }

        var active = QuestManager.Instance.GetActiveQuests();

        if (active.Count == 0)
        {
            _text.text = "";
            return;
        }

        var builder = new StringBuilder();

        foreach (Quest quest in active)
        {
            QuestProgressData progress = QuestManager.Instance.GetProgress(quest.Id);
            builder.AppendLine($"{quest.Title}: {progress.CurrentCount}/{quest.TargetCount}");
        }

        _text.text = builder.ToString().TrimEnd();
    }

    private void Build()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        _panel = new GameObject("Panel", typeof(RectTransform));
        _panel.transform.SetParent(transform, false);
        var panel = _panel;

        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -20f);
        panelRect.sizeDelta = new Vector2(520f, 160f);

        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.55f);
        panelImage.raycastTarget = false;

        var textGo = new GameObject("Text", typeof(RectTransform));
        textGo.transform.SetParent(panel.transform, false);

        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 8f);
        textRect.offsetMax = new Vector2(-12f, -8f);

        _text = textGo.AddComponent<Text>();
        _text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _text.fontSize = 22;
        _text.color = Color.white;
        _text.alignment = TextAnchor.UpperCenter;
        _text.horizontalOverflow = HorizontalWrapMode.Wrap;
        _text.verticalOverflow = VerticalWrapMode.Overflow;
    }
}
