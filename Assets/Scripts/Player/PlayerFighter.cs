using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Отвечает за боевую механику игрока, включая атаки, получение урона и смерть.
/// </summary>
public class PlayerFighter : Fighter
{
    private const string FirstLevelName = "FirstLevel";

    /// <summary> Менеджер экипировки игрока. </summary>
    [SerializeField] private EqupimentManager _equipmentManager;

    /// <summary> Контроллер смены сцен. </summary>
    [SerializeField] private SceneController _sceneController;

    /// <summary> Инвентарь игрока (временно). </summary>
    [SerializeField] private Inventory _inventory;
    
    [SerializeField] private EqupimentManager _equipment;

    /// <summary> Задержка перед перезапуском уровня после смерти. </summary>
    [SerializeField] private float _delay = 3f;

    [SerializeField] private float _blockDamageMultiplier = 0.25f;

    private WaitForSeconds _waitForSeconds;
    private bool _canPress = true;
    private bool _isInvulnerable;

    public event Action Attacked;
    public event Action Died;

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(_delay);
    }

    private void Update()
    {
        IsBlocking = IsDead == false
                     && _isInvulnerable == false
                     && _equipmentManager.Shield != null
                     && Input.GetKey(KeyCode.LeftControl);

        if (IsCooldown || _canPress == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attacked?.Invoke();
            _canPress = false;
        }
    }

    public void SetInvulnerable(bool value)
    {
        _isInvulnerable = value;
    }

    public bool TryConsume(Item item)
    {
        if (item == null || item.IsConsumable == false || IsDead)
        {
            return false;
        }

        Health.Heal(item.HealAmount);
        return true;
    }

    protected override float GetModifiedDamage(float damage)
    {
        if (_isInvulnerable)
        {
            return 0f;
        }

        if (IsBlocking)
        {
            return damage * _blockDamageMultiplier;
        }

        return damage;
    }

    protected override void OnDied()
    {
        Died?.Invoke();
        StartCoroutine(DeathDelay());
    }

    /// <summary>
    /// Вычисляет общий урон игрока, включая бонусы от экипированного оружия.
    /// </summary>
    /// <returns>Общий урон игрока.</returns>
    protected override float GetTotalDamage()
    {
        Debug.Log("Attack");
        float baseDamage = base.GetTotalDamage();
        float weaponBonus = _equipmentManager.Weapon != null ? _equipmentManager.Weapon.Damage : 0;
        return baseDamage + weaponBonus;
    }

    /// <summary>
    /// Вычисляет общую защиту игрока, включая бонусы от брони, щита и шлема.
    /// </summary>
    /// <returns>Общая защита игрока.</returns>
    protected override float GetTotalDefense()
    {
        float baseDefense = base.GetTotalDefense();
        float armorBonus = _equipmentManager.Armor != null ? _equipmentManager.Armor.Def : 0;
        float shieldBonus = _equipmentManager.Shield != null ? _equipmentManager.Shield.Def : 0;
        float helmetBonus = _equipmentManager.Helmet != null ? _equipmentManager.Helmet.Def : 0;
        return baseDefense + armorBonus + shieldBonus + helmetBonus;
    }

    /// <summary>
    /// Выполняет атаку по противнику, если он находится в зоне удара.
    /// </summary>
    private void PreformAttack()
    {
        Debug.Log("Preform");
        
        if (InTrigger && CollidedObject.TryGetComponent(out EnemyFighter enemyFighter))
        {
            Debug.Log("Preform Attack");
            Attack(enemyFighter);
        }

        _canPress = true;
    }

    /// <summary>
    /// Задержка перед перезапуском уровня после смерти.
    /// </summary>
    private IEnumerator DeathDelay()
    {
        yield return _waitForSeconds;

        _equipmentManager.Clear();
        _sceneController.LoadScene(FirstLevelName, _inventory, _equipment);
    }
}
