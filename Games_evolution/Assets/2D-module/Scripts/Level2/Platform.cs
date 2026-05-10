using UnityEngine;
using System.Collections;

 
public class Platform : MonoBehaviour
    {
        [SerializeField] private float disappearDelay = 5f;
        private Coroutine vanishRoutine;
        private PlatformGenerator generator;
        private int platformTypeIndex;
        private bool playerOnPlatform = false;

        public int PlatformTypeIndex => platformTypeIndex;

        public void Init(PlatformGenerator gen, int typeIndex)
        {
            generator = gen;
            platformTypeIndex = typeIndex;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerOnPlatform = true;
                if (vanishRoutine != null) StopCoroutine(vanishRoutine);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerOnPlatform = false;
                vanishRoutine = StartCoroutine(VanishAfterDelay());
            }
        }

        IEnumerator VanishAfterDelay()
        {
            yield return new WaitForSeconds(disappearDelay);
            if (!playerOnPlatform)
            {
                // Возвращаем в пул
                if (generator != null)
                {
                    generator.ReturnPlatformToPool(gameObject, platformTypeIndex);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        // Вызывается из генератора при возврате
        public void ReturnToPool()
        {
            if (vanishRoutine != null) StopCoroutine(vanishRoutine);
            gameObject.SetActive(false);
        }
    }
 