using UnityEngine;
using System.Collections;

public class BreakablePlatform : MonoBehaviour
{
    private bool isBroken = false;
    private SpriteRenderer sr;
    private Collider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isBroken)
        {
            StartCoroutine(BreakAndRespawn());
        }
    }

    IEnumerator BreakAndRespawn()
    {
        isBroken = true;
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(3f);

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
        isBroken = false;
    }
}