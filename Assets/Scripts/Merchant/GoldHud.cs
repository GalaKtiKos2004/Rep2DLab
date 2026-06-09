using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображает количество золота в углу экрана.
/// </summary>
public class GoldHud : MonoBehaviour
{
    private static GoldHud _instance;

    private Text _goldText;

    public static void EnsureExists()
    {
        if (_instance != null)
        {
            _instance.Refresh();
            return;
        }

        var root = new GameObject("GoldHud");
        _instance = root.AddComponent<GoldHud>();
        _instance.Build();
        DontDestroyOnLoad(root);
    }

    private void OnEnable()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.GoldChanged += OnGoldChanged;
            Refresh();
        }
    }

    private void OnDisable()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.GoldChanged -= OnGoldChanged;
        }
    }

    private void OnGoldChanged(int gold)
    {
        if (_goldText != null)
        {
            _goldText.text = $"Золото: {gold}";
        }
    }

    private void Refresh()
    {
        if (GameSession.Instance != null)
        {
            OnGoldChanged(GameSession.Instance.Gold);
        }
    }

    private void Build()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        var panel = CreateChild("Panel", transform);
        var panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.55f);
        panel.anchorMin = new Vector2(1f, 1f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.pivot = new Vector2(1f, 1f);
        panel.anchoredPosition = new Vector2(-20f, -20f);
        panel.sizeDelta = new Vector2(220f, 48f);

        var textRect = CreateChild("Text", panel);
        Stretch(textRect, 12f, 12f, 8f, 8f);

        _goldText = textRect.gameObject.AddComponent<Text>();
        _goldText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _goldText.fontSize = 22;
        _goldText.color = new Color(1f, 0.85f, 0.2f);
        _goldText.alignment = TextAnchor.MiddleCenter;

        Refresh();
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
