using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] private GameObject portalObject;   // перетащи сюда объект портала
    private bool portalSpawned;

    void Update()
    {
        if (!portalSpawned && PlatformerController.Instance != null
            && PlatformerController.Instance.EnemiesKilled >= 5)
        {
            portalSpawned = true;
            portalObject.SetActive(true);
            UnifiedInfoSystem.Instance.UnlockArticle("platformer_portal");
        }
    }
}