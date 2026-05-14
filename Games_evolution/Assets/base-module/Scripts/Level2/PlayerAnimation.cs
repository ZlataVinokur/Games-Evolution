using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        // Находим компоненты на персонаже
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        bool isWalking = Mathf.Abs(moveInput) > 0.1f;
        if (isWalking)
            Debug.Log("Должна быть анимация ходьбы, moveInput = " + moveInput);
        animator.SetBool("isWalking", isWalking);
       
    }
}