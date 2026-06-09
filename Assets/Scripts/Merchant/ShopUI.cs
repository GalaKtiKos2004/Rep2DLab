using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Интерфейс магазина: покупка и продажа предметов.
/// </summary>
public class ShopUI : MonoBehaviour
{
    private enum ShopMode
    {
        Buy,
        Sell
    }

    private Inventory _inventory;
    private EqupimentManager _equipment;
    private Action _onClosed;

    private List<Item> _wares = new List<Item>();
    private List<Item> _legendaryWares = new List<Item>();
    private string _legendaryBuyQuestId;
    private ShopMode _mode = ShopMode.Buy;

    private Text _titleText;
    private Text _goldText;
    private Text _descriptionText;
    private Text _actionLabelText;
    private Button _actionButton;
    private Button _buyTabButton;
    private Button _sellTabButton;
    private RectTransform _listContent;
    private Font _font;

    private Item _selectedBuyItem;
    private ItemSlot _selectedSellSlot;

    public static void Open(
        string merchantName,
        IReadOnlyList<Item> wares,
        IReadOnlyList<Item> legendaryWares,
        Inventory inventory,
        EqupimentManager equipment,
        string legendaryBuyQuestId,
        Action onClosed)
    {
        var root = new GameObject("ShopUI");
        var shop = root.AddComponent<ShopUI>();
        shop._inventory = inventory;
        shop._equipment = equipment;
        shop._onClosed = onClosed;
        shop._wares = new List<Item>(wares);
        shop._legendaryWares = new List<Item>(legendaryWares);
        shop._legendaryBuyQuestId = legendaryBuyQuestId;
        shop.Build(merchantName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    private void OnEnable()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.GoldChanged += OnGoldChanged;
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.QuestsChanged += OnQuestsChanged;
        }
    }

    private void OnDisable()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.GoldChanged -= OnGoldChanged;
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.QuestsChanged -= OnQuestsChanged;
        }
    }

    private void OnQuestsChanged()
    {
        RebuildList();
        RefreshActionButton();
    }

    private void OnGoldChanged(int gold)
    {
        if (_goldText != null)
        {
            _goldText.text = $"Золото: {gold}";
        }

        RefreshActionButton();
    }

    private void Build(string merchantName)
    {
        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 160;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        var panel = CreateChild("Panel", transform);
        var panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.06f, 0.04f, 0.95f);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(1040f, 720f);

        var header = CreateChild("Header", panel);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, -24f);
        header.sizeDelta = new Vector2(-48f, 52f);

        _titleText = CreateText("Title", header, merchantName, 32, TextAnchor.MiddleLeft);
        Stretch(_titleText.rectTransform, 0f, 340f, 0f, 0f);

        _goldText = CreateText("Gold", header, "Золото: 0", 28, TextAnchor.MiddleRight);
        Stretch(_goldText.rectTransform, 340f, 0f, 0f, 0f);
        _goldText.color = new Color(1f, 0.85f, 0.2f);

        var tabs = CreateChild("Tabs", panel);
        tabs.anchorMin = new Vector2(0f, 1f);
        tabs.anchorMax = new Vector2(1f, 1f);
        tabs.pivot = new Vector2(0.5f, 1f);
        tabs.anchoredPosition = new Vector2(0f, -88f);
        tabs.sizeDelta = new Vector2(-48f, 44f);

        _buyTabButton = CreateTabButton(tabs, "Купить", 0f, () => SetMode(ShopMode.Buy));
        _sellTabButton = CreateTabButton(tabs, "Продать", 180f, () => SetMode(ShopMode.Sell));

        var listArea = CreateChild("ListArea", panel);
        listArea.anchorMin = new Vector2(0f, 0f);
        listArea.anchorMax = new Vector2(1f, 1f);
        listArea.offsetMin = new Vector2(24f, 140f);
        listArea.offsetMax = new Vector2(-24f, -132f);

        var listBackground = listArea.gameObject.AddComponent<Image>();
        listBackground.color = new Color(0f, 0f, 0f, 0.35f);

        var scroll = CreateChild("Scroll", listArea);
        Stretch(scroll, 8f, 8f, 8f, 8f);

        var scrollRect = scroll.gameObject.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;

        _listContent = CreateChild("Content", scroll);
        _listContent.anchorMin = new Vector2(0f, 1f);
        _listContent.anchorMax = new Vector2(1f, 1f);
        _listContent.pivot = new Vector2(0.5f, 1f);
        _listContent.anchoredPosition = Vector2.zero;

        var layout = _listContent.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.spacing = 8f;
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = _listContent.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = _listContent;
        scrollRect.viewport = scroll;

        var footer = CreateChild("Footer", panel);
        footer.anchorMin = new Vector2(0f, 0f);
        footer.anchorMax = new Vector2(1f, 0f);
        footer.pivot = new Vector2(0.5f, 0f);
        footer.anchoredPosition = new Vector2(0f, 24f);
        footer.sizeDelta = new Vector2(-48f, 110f);

        _descriptionText = CreateText("Description", footer, "Выберите предмет", 24, TextAnchor.UpperLeft);
        Stretch(_descriptionText.rectTransform, 0f, 300f, 0f, 0f);

        _actionButton = CreateButton("Action", footer, "Купить", new Vector2(1f, 0.5f), new Vector2(220f, 48f), OnActionClicked);
        _actionButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120f, 4f);

        _actionLabelText = _actionButton.GetComponentInChildren<Text>();

        var closeButton = CreateButton("Close", footer, "Закрыть (Esc)", new Vector2(1f, 0.5f), new Vector2(220f, 48f), Close);
        closeButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120f, -52f);

        OnGoldChanged(GameSession.Instance != null ? GameSession.Instance.Gold : 0);
        SetMode(ShopMode.Buy);
    }

    private void SetMode(ShopMode mode)
    {
        _mode = mode;
        _selectedBuyItem = null;
        _selectedSellSlot = null;
        _descriptionText.text = "Выберите предмет";
        RefreshTabs();
        RebuildList();
        RefreshActionButton();
    }

    private void RefreshTabs()
    {
        SetTabState(_buyTabButton, _mode == ShopMode.Buy);
        SetTabState(_sellTabButton, _mode == ShopMode.Sell);
    }

    private void SetTabState(Button button, bool active)
    {
        var colors = button.colors;
        colors.normalColor = active ? new Color(0.45f, 0.32f, 0.12f) : new Color(0.2f, 0.2f, 0.2f);
        button.colors = colors;
    }

    private void RebuildList()
    {
        for (int i = _listContent.childCount - 1; i >= 0; i--)
        {
            Destroy(_listContent.GetChild(i).gameObject);
        }

        if (_mode == ShopMode.Buy)
        {
            BuildBuyList();
        }
        else
        {
            BuildSellList();
        }
    }

    private void BuildBuyList()
    {
        foreach (var item in _wares)
        {
            if (item == null)
            {
                continue;
            }

            CreateBuyListButton(item, false);
        }

        foreach (var item in _legendaryWares)
        {
            if (item == null)
            {
                continue;
            }

            CreateBuyListButton(item, IsLegendaryBuyUnlocked() == false);
        }
    }

    private void BuildSellList()
    {
        int itemCount = 0;

        foreach (var slot in _inventory.ItemSlots)
        {
            if (slot.IsEmpty)
            {
                continue;
            }

            Item item = slot.ItemInSlot;
            string label = $"{item.Name} — {item.GetSellPrice()} з.";
            CreateListButton(label, item, slot);
            itemCount++;
        }

        if (_equipment != null)
        {
            foreach (var slot in _equipment.GetOccupiedEquipmentSlots())
            {
                Item item = slot.ItemInSlot;
                string label = $"{item.Name} (надето) — {item.GetSellPrice()} з.";
                CreateListButton(label, item, slot);
                itemCount++;
            }
        }

        if (itemCount == 0)
        {
            CreateInfoRow("Нет предметов для продажи. Положи вещи в инвентарь (I).");
        }
    }

    private void CreateBuyListButton(Item item, bool locked)
    {
        string lockLabel = locked ? " [нужен квест]" : "";
        string label = $"{item.Name}{lockLabel} — {item.GetBuyPrice()} з.";
        CreateListButton(label, item, null, locked);
    }

    private bool IsLegendaryBuyUnlocked()
    {
        return QuestManager.Instance != null
            && QuestManager.Instance.IsQuestCompleted(_legendaryBuyQuestId);
    }

    private bool CanBuyItem(Item item)
    {
        if (item == null || _legendaryWares.Contains(item) == false)
        {
            return true;
        }

        return IsLegendaryBuyUnlocked();
    }

    private string GetLegendaryBuyLockMessage()
    {
        Quest quest = QuestManager.Instance != null
            ? QuestManager.Instance.FindQuest(_legendaryBuyQuestId)
            : null;

        string questTitle = quest != null ? quest.Title : "нужный";
        return $"Легендарное оружие можно купить только после выполнения квеста «{questTitle}».";
    }

    private void CreateListButton(string label, Item item, ItemSlot slot, bool locked = false)
    {
        var row = CreateChild("Row", _listContent);
        var rowLayout = row.gameObject.AddComponent<LayoutElement>();
        rowLayout.minHeight = 44f;
        rowLayout.preferredHeight = 44f;

        var button = row.gameObject.AddComponent<Button>();
        var image = row.gameObject.AddComponent<Image>();
        image.color = locked
            ? new Color(0.14f, 0.12f, 0.12f, 0.75f)
            : new Color(0.18f, 0.16f, 0.14f, 0.95f);

        var text = CreateText("Label", row, label, 22, TextAnchor.MiddleLeft);
        Stretch(text.rectTransform, 16f, 16f, 6f, 6f);

        if (locked)
        {
            text.color = new Color(0.65f, 0.65f, 0.65f);
        }

        button.onClick.AddListener(() =>
        {
            if (_mode == ShopMode.Buy)
            {
                _selectedBuyItem = item;
                _selectedSellSlot = null;

                if (CanBuyItem(item))
                {
                    _descriptionText.text = $"{item.Name}\n{item.Description}\nЦена: {item.GetBuyPrice()} з.";
                }
                else
                {
                    _descriptionText.text = $"{item.Name}\n{item.Description}\n{GetLegendaryBuyLockMessage()}";
                }
            }
            else
            {
                _selectedSellSlot = slot;
                _selectedBuyItem = null;
                _descriptionText.text = $"{item.Name}\n{item.Description}\nПродажа: {item.GetSellPrice()} з.";
            }

            RefreshActionButton();
        });
    }

    private void RefreshActionButton()
    {
        if (_mode == ShopMode.Buy)
        {
            _actionLabelText.text = "Купить";
            bool canBuy = _selectedBuyItem != null
                && CanBuyItem(_selectedBuyItem)
                && GameSession.Instance != null
                && GameSession.Instance.CanAfford(_selectedBuyItem.GetBuyPrice())
                && _inventory.HasFreeSlot();
            _actionButton.interactable = canBuy;
            return;
        }

        _actionLabelText.text = "Продать";
        _actionButton.interactable = _selectedSellSlot != null && _selectedSellSlot.IsEmpty == false;
    }

    private void OnActionClicked()
    {
        if (_mode == ShopMode.Buy)
        {
            TryBuySelected();
        }
        else
        {
            TrySellSelected();
        }
    }

    private void TryBuySelected()
    {
        if (_selectedBuyItem == null || GameSession.Instance == null)
        {
            return;
        }

        if (CanBuyItem(_selectedBuyItem) == false)
        {
            _descriptionText.text = GetLegendaryBuyLockMessage();
            return;
        }

        int price = _selectedBuyItem.GetBuyPrice();

        if (GameSession.Instance.CanAfford(price) == false)
        {
            _descriptionText.text = "Недостаточно золота.";
            return;
        }

        if (_inventory.HasFreeSlot() == false)
        {
            _descriptionText.text = "Инвентарь полон.";
            return;
        }

        if (GameSession.Instance.TrySpendGold(price) == false)
        {
            return;
        }

        _inventory.AddItem(_selectedBuyItem);
        _descriptionText.text = $"Куплено: {_selectedBuyItem.Name}";
        RefreshActionButton();
    }

    private void TrySellSelected()
    {
        if (_selectedSellSlot == null || _selectedSellSlot.IsEmpty || GameSession.Instance == null)
        {
            return;
        }

        Item item = _selectedSellSlot.ItemInSlot;
        int price = item.GetSellPrice();
        _inventory.SellItemFromSlot(_selectedSellSlot);
        GameSession.Instance.AddGold(price);

        _selectedSellSlot = null;
        _descriptionText.text = $"Продано: {item.Name} (+{price} з.)";
        RebuildList();
        RefreshActionButton();
    }

    private void Close()
    {
        _onClosed?.Invoke();
        Destroy(gameObject);
    }

    private RectTransform CreateChild(string name, Transform parent)
    {
        var child = new GameObject(name, typeof(RectTransform));
        child.transform.SetParent(parent, false);
        return child.GetComponent<RectTransform>();
    }

    private Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment)
    {
        var rect = CreateChild(name, parent);
        var text = rect.gameObject.AddComponent<Text>();
        text.font = _font;
        text.text = value;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private Button CreateTabButton(Transform parent, string label, float xOffset, Action onClick)
    {
        var rect = CreateChild(label + "Tab", parent);
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(xOffset, 0f);
        rect.sizeDelta = new Vector2(160f, 42f);

        var image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.28f, 0.22f, 0.14f);

        var button = rect.gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => onClick());

        var text = CreateText("Label", rect, label, 22, TextAnchor.MiddleCenter);
        Stretch(text.rectTransform, 10f, 10f, 6f, 6f);

        return button;
    }

    private void CreateInfoRow(string message)
    {
        var row = CreateChild("Info", _listContent);
        var rowLayout = row.gameObject.AddComponent<LayoutElement>();
        rowLayout.minHeight = 58f;
        rowLayout.preferredHeight = 58f;

        var text = CreateText("Label", row, message, 22, TextAnchor.MiddleLeft);
        Stretch(text.rectTransform, 16f, 16f, 10f, 10f);
        text.color = new Color(0.8f, 0.8f, 0.8f);
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 anchor, Vector2 size, Action onClick)
    {
        var rect = CreateChild(name, parent);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;

        var image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.28f, 0.22f, 0.14f);

        var button = rect.gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => onClick());

        var text = CreateText("Label", rect, label, 22, TextAnchor.MiddleCenter);
        Stretch(text.rectTransform, 10f, 10f, 6f, 6f);

        return button;
    }

    private static void Stretch(RectTransform rect, float left, float right, float top, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }
}
