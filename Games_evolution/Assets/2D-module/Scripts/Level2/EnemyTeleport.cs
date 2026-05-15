using UnityEngine;
using System.Collections;

public class EnemyTeleport : Enemy_2
{
    public float teleportInterval = 2f;
    public float teleportRadius = 3f;

    void Start()
    {
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);
            Vector3 newPos = transform.position + (Vector3)Random.insideUnitCircle * teleportRadius;
            newPos.y = Mathf.Clamp(newPos.y, transform.position.y - 2f, transform.position.y + 2f);
            transform.position = newPos;
        }
    }
}