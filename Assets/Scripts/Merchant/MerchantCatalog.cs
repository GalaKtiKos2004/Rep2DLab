using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MerchantCatalog", menuName = "Merchant/Catalog")]
public class MerchantCatalog : ScriptableObject
{
    [Serializable]
    public class MerchantLevelSpawn
    {
        public string SceneName;
        public Vector3 SpawnPosition = new Vector3(-0.72f, 0.08f, 0f);

        [Tooltip("Пусто — используется общий LegendaryBuyQuestId каталога")]
        public string LegendaryBuyQuestId;
    }

    public string MerchantName = "Торговец";
    public Sprite MerchantSprite;
    public List<Item> Wares = new List<Item>();

    [Tooltip("Легендарное оружие, доступное к покупке после квеста")]
    public List<Item> LegendaryWares = new List<Item>();

    public List<MerchantLevelSpawn> LevelSpawns = new List<MerchantLevelSpawn>();

    [Tooltip("ID квеста по умолчанию для покупки легендарного оружия")]
    public string LegendaryBuyQuestId = "clear_outskirts";

    public bool TryGetLevelSpawn(string sceneName, out Vector3 spawnPosition, out string legendaryBuyQuestId)
    {
        foreach (MerchantLevelSpawn levelSpawn in LevelSpawns)
        {
            if (levelSpawn == null || levelSpawn.SceneName != sceneName)
            {
                continue;
            }

            spawnPosition = levelSpawn.SpawnPosition;
            legendaryBuyQuestId = string.IsNullOrEmpty(levelSpawn.LegendaryBuyQuestId)
                ? LegendaryBuyQuestId
                : levelSpawn.LegendaryBuyQuestId;
            return true;
        }

        spawnPosition = default;
        legendaryBuyQuestId = null;
        return false;
    }
}
