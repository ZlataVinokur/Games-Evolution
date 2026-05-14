using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        // Стандартный паттерн Singleton: если экземпляр ещё не создан, этот объект становится главным.
        // Также проверяем наличие компонента AudioSource
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Убеждаемся, что AudioSource присутствует
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}