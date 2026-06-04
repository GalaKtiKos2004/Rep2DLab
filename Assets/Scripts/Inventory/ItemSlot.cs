using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private InventoryUi _inventoryUi;

    private Item _item;
    private Image _image;
    private Button _button;
    private Transform _originalParent;
    private Vector3 _startTransform;
    private CanvasGroup _canvasGroup;

    public Item ItemInSlot => _item;
    public bool IsEmpty => _item == null;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    public void SetItem(Item item)
    {
        _item = item;
        _image.sprite = item?.icon;

        if (_item != null)
        {
            _image.color = new Vector4(_image.color.r, _image.color.g, _image.color.b, 1f);
        }
        else
        {
            Clear();
        }
    }

    public void Clear()
    {
        _item = null;
        _image.sprite = null;
        _image.color = new Vector4(_image.color.r, _image.color.g, _image.color.b, 0f);
    }

    public void OnClick()
    {
        if (_item != null)
        {
            _inventoryUi.SelectItem(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty) return;

        _startTransform = transform.position;
        _originalParent = transform.parent;
        transform.SetParent(_originalParent.root, true);
        transform.SetAsLastSibling();
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsEmpty) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        transform.SetParent(_originalParent, true);
        transform.position = _startTransform;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (eventData.pointerDrag.TryGetComponent(out ItemSlot draggedSlot) && draggedSlot != this)
        {
            SwapItems(draggedSlot);
        }
    }

    private void SwapItems(ItemSlot targetSlot)
    {
        if (targetSlot == null || targetSlot == this) return;

        Item tempItem = targetSlot._item;
        targetSlot.SetItem(_item);
        SetItem(tempItem);
    }
}
