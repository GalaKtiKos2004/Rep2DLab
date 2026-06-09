using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC-торговец: открывает магазин по клавише E рядом с триггером.
/// </summary>
public class MerchantNpc : MonoBehaviour
{
    private const KeyCode TalkKey = KeyCode.E;

    [SerializeField] private Trigger _trigger;
    [SerializeField] private string _merchantName = "Торговец";
    [SerializeField] private List<Item> _wares = new List<Item>();
    [SerializeField] private List<Item> _legendaryWares = new List<Item>();
    [SerializeField] private string _legendaryBuyQuestId;

    private bool _playerNearby;
    private bool _shopOpen;

    public void Setup(Trigger trigger, MerchantCatalog catalog, string legendaryBuyQuestId)
    {
        UnsubscribeFromTrigger();
        _trigger = trigger;
        _merchantName = catalog.MerchantName;
        _wares = new List<Item>(catalog.Wares);
        _legendaryWares = new List<Item>(catalog.LegendaryWares);
        _legendaryBuyQuestId = legendaryBuyQuestId;
        SubscribeToTrigger();
    }

    private void OnEnable()
    {
        SubscribeToTrigger();
    }

    private void OnDisable()
    {
        UnsubscribeFromTrigger();
    }

    private void SubscribeToTrigger()
    {
        if (_trigger == null)
        {
            return;
        }

        _trigger.TriggerEntered -= OnPlayerEntered;
        _trigger.TriggerExited -= OnPlayerExited;
        _trigger.TriggerEntered += OnPlayerEntered;
        _trigger.TriggerExited += OnPlayerExited;
    }

    private void UnsubscribeFromTrigger()
    {
        if (_trigger == null)
        {
            return;
        }

        _trigger.TriggerEntered -= OnPlayerEntered;
        _trigger.TriggerExited -= OnPlayerExited;
    }

    private void Update()
    {
        if (_shopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
            return;
        }

        if (_playerNearby == false || _shopOpen || Input.GetKeyDown(TalkKey) == false)
        {
            return;
        }

        OpenShop();
    }

    private void OpenShop()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        EqupimentManager equipment = FindObjectOfType<EqupimentManager>();

        if (inventory == null)
        {
            return;
        }

        _shopOpen = true;
        ShopUI.Open(
            _merchantName,
            _wares,
            _legendaryWares,
            inventory,
            equipment,
            _legendaryBuyQuestId,
            CloseShop);
    }

    private void CloseShop()
    {
        _shopOpen = false;
    }

    private void OnPlayerEntered(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            _playerNearby = true;
        }
    }

    private void OnPlayerExited(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            _playerNearby = false;
        }
    }

    private static bool IsPlayer(Collider2D collision)
    {
        return collision.GetComponentInParent<PlayerMover>() != null;
    }
}
