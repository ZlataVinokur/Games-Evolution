using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Загрузка первого уровня
    public void StartGame()
    {
        Debug.Log("Загрузка уровня 1...");
        SceneManager.LoadScene("1_Level1");
    }
    
    // Выход из игры
    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    // Загрузка любого уровня по индексу
    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
    
    // Загрузка уровня по имени
    public void LoadLevelByName(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

}