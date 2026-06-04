using UnityEngine;

public class SuperPellet : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f; // градусов в секунду

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}