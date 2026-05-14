using UnityEngine;

public class Portal_2 : MonoBehaviour
{


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CompleteLevel("Level2_Platformer_2");
            SceneLoader.LoadScene("Level3_RPG");
        }
    }
}