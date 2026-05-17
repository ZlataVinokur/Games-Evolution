using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($" GameEvents инициализирован на объекте {gameObject.name}");
    }

    public void OnGameWin()
    {

        GameManager.Instance.LoadQuizForCurrentModule(8);

    }
}