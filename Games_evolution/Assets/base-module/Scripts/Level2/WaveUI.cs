using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WaveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;          // ссылка на текстовое поле
    [SerializeField] private string wavePrefix = "ВОЛНА: "; // например "Wave: "

    public void UpdateWave(int currentWave, int totalWaves)
    {
        if (waveText != null)
        {
            waveText.text = wavePrefix + currentWave + " / " + totalWaves;
        }
    }
}