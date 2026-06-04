using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отвечает за отображение и управление интерфейсом инвентаря.
/// Позволяет выбирать, использовать и выбрасывать предметы.
/// </summary>
public class InventoryUi : MonoBehaviour
{
    /// <summary> Ссылка на инвентарь игрока. </summary>
    [SerializeField] private Inventory _inventory;

    /// <summary> Ссылка на менеджер экипировки, который отвечает за снаряжение предметов. </summary>
    [SerializeField] private EqupimentManager _equipmentManager;

    /// <summary> Кнопка использования предмета. </summary>
    [SerializeField] private Button _useButton;

    /// <summary> Кнопка выброса предмета. </summary>
    [SerializeField] private Button _dropButton;

    /// <summary> Поле текста для отображения описания выбранного предмета. </summary>
    [SerializeField] private TextMeshProUGUI _descriptionText;

    /// <summary> Изображение инвентаря (иконка или фон). </summary>
    [SerializeField] private Image _inventoryImage;

    /// <summary> CanvasGroup, управляющий видимостью и взаимодействием с UI инвентаря. </summary>
    [SerializeField] private CanvasGroup _canvasGroup;

    private Item _selectedItem;
    private ItemSlot _selectedItemSlot;
    private bool _isActive = false;

    /// <summary>
    /// Подписывается на события кнопок при активации объекта.
    /// </summary>
    private void OnEnable()
    {
        _useButton.onClick.AddListener(UseItem);
        _dropButton.onClick.AddListener(DropItem);
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Обрабатывает нажатие клавиши "I" для открытия и закрытия инвентаря.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _isActive = !_isActive;
            _canvasGroup.alpha = _isActive ? 1 : 0;
            _canvasGroup.blocksRaycasts = _isActive;
            _useButton.interactable = _isActive;
            _dropButton.interactable = _isActive;
        }
    }

    /// <summary>
    /// Выбирает предмет в инвентаре и обновляет описание.
    /// </summary>
    /// <param name="item">Выбранный слот предмета.</param>
    public void SelectItem(ItemSlot item)
    {
        _selectedItemSlot = item;
        _selectedItem = item.ItemInSlot;
        _descriptionText.text = _selectedItem.Description;
    }

    /// <summary>
    /// Использует выбранный предмет, удаляя его из инвентаря и экипируя персонажу.
    /// </summary>
    private void UseItem()
    {
        if (_selectedItem != null)
        {
            _inventory.RemoveItemFromSlot(_selectedItemSlot);
            _equipmentManager.EquipItem(_selectedItem);
        }
    }

    /// <summary>
    /// Выбрасывает выбранный предмет из инвентаря.
    /// </summary>
    private void DropItem()
    {
        Debug.Log("drop item");
        if (_selectedItem != null)
        {
            _inventory.RemoveItemFromSlot(_selectedItemSlot);
            _selectedItem = null;
            _descriptionText.text = "";
        }
    }
}
