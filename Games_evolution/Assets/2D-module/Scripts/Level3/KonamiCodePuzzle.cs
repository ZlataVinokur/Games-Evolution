using System.Collections.Generic;
using UnityEngine;

public class KonamiCodePuzzle : MonoBehaviour
{
    private List<KeyCode> sequence = new List<KeyCode> { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A };
    private int currentIndex = 0;

    void Update()
    {
        if (RPGLevelManager.Instance.metersActivated[1]) return;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    if (code == sequence[currentIndex])
                    {
                        currentIndex++;
                        if (currentIndex >= sequence.Count)
                        {
                            RPGLevelManager.Instance.ActivateMeter(1);
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        currentIndex = 0;
                    }
                    break;
                }
            }
        }
    }
}