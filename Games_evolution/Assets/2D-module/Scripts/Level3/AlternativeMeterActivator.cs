using UnityEngine;

public class AlternativeMeterActivator : MonoBehaviour
{
    public int requiredKills = 5;
    private int killCount = 0;

    void Start()
    {
        HazardBug.OnAnyBugDeath += OnBugDeath;
    }

    void OnDestroy()
    {
        HazardBug.OnAnyBugDeath -= OnBugDeath;
    }

    void OnBugDeath()
    {
        if (RPGLevelManager.Instance == null) return;
        if (RPGLevelManager.Instance.metersActivated[2])
        {
            Debug.Log("«еленый измеритель уже активен, убийства багов не нужны");
            return;
        }

        killCount++;
        Debug.Log($"”бито багов: {killCount}/{requiredKills}");
        if (killCount >= requiredKills)
        {
            Debug.Log("ѕопытка активировать измеритель 2 через убийство багов");
            RPGLevelManager.Instance.ActivateMeter(2);
        }
    }
}