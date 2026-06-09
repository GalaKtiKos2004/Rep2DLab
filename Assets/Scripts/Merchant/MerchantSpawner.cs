using UnityEngine;

/// <summary>
/// Создаёт торговца на уровне по каталогу из Resources.
/// </summary>
public static class MerchantSpawner
{
    private static readonly string[] SkippedScenes = { "GameStart", "GameFinish" };

    private const string VillageMerchantPath = "Merchant/VillageMerchant";

    public static void SpawnForScene(string sceneName)
    {
        if (ShouldSkip(sceneName))
        {
            return;
        }

        MerchantCatalog catalog = Resources.Load<MerchantCatalog>(VillageMerchantPath);

        if (catalog == null)
        {
            Debug.LogWarning("Каталог торговца не найден в Resources/Merchant/VillageMerchant");
            return;
        }

        if (catalog.TryGetLevelSpawn(sceneName, out Vector3 spawnPosition, out string legendaryBuyQuestId) == false)
        {
            return;
        }

        MerchantNpc existingMerchant = Object.FindObjectOfType<MerchantNpc>();

        if (existingMerchant != null)
        {
            Object.Destroy(existingMerchant.gameObject);
        }

        var merchantObject = new GameObject(catalog.MerchantName);
        merchantObject.transform.position = spawnPosition;

        var spriteRenderer = merchantObject.AddComponent<SpriteRenderer>();

        if (catalog.MerchantSprite != null)
        {
            spriteRenderer.sprite = catalog.MerchantSprite;
            spriteRenderer.color = new Color(0.85f, 0.7f, 0.45f);
        }

        spriteRenderer.sortingOrder = 1;

        var bodyCollider = merchantObject.AddComponent<BoxCollider2D>();
        bodyCollider.size = new Vector2(0.18f, 0.18f);

        var triggerObject = new GameObject("Trigger");
        triggerObject.transform.SetParent(merchantObject.transform);
        triggerObject.transform.localPosition = Vector3.zero;

        var triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector2(0.55f, 0.55f);

        var trigger = triggerObject.AddComponent<Trigger>();

        var triggerBody = triggerObject.AddComponent<Rigidbody2D>();
        triggerBody.bodyType = RigidbodyType2D.Kinematic;
        triggerBody.gravityScale = 0f;

        var merchant = merchantObject.AddComponent<MerchantNpc>();
        merchant.Setup(trigger, catalog, legendaryBuyQuestId);
    }

    private static bool ShouldSkip(string sceneName)
    {
        foreach (string skipped in SkippedScenes)
        {
            if (skipped == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}
