using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private bool triggerOnce = true;

    private bool triggered = false;

    private void Start()
    {
        // Проверяем, что коллайдер настроен как триггер
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($" {gameObject.name}: Нет коллайдера! Добавьте Collider и включите Is Trigger.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogError($" {gameObject.name}: Коллайдер не Is Trigger! Включите галочку Is Trigger в инспекторе.");
        }
        else
        {
            Debug.Log($" {gameObject.name}: Триггер настроен правильно.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($" {gameObject.name}: Что-то вошло в триггер: {other.gameObject.name}, тег: {other.tag}");

        if (triggerOnce && triggered)
        {
            Debug.Log($" {gameObject.name}: Уже срабатывал (triggerOnce = true), пропускаем.");
            return;
        }

        if (other.CompareTag(playerTag))
        {
            Debug.Log($" {gameObject.name}: Игрок вошел в триггер! Вызываем событие.");
            triggered = true;
            onTriggerEnter?.Invoke();
        }
        else
        {
            Debug.Log($" {gameObject.name}: Тег {other.tag} не совпадает с ожидаемым {playerTag}.");
        }
    }

    // Визуализация триггера в редакторе
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
        }
    }
}