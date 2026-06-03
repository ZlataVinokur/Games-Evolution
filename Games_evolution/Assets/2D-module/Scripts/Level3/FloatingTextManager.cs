using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    [Header("Prefab")]
    public GameObject floatingTextPrefab; // TextMeshPro текст

    void Awake() => Instance = this;

    public void ShowDamage(Vector3 worldPosition, int damage, bool isHeal = false)
    {
        if (floatingTextPrefab == null) return;
        GameObject go = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity);
        TextMeshPro tmp = go.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = damage.ToString();
            tmp.color = isHeal ? Color.green : Color.red;
        }
        Destroy(go, 1f);
    }
}