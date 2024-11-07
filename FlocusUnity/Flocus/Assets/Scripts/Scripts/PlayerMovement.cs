using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Vector2 movementInput;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Get input
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        // Calculate the target position
        Vector2 targetPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;

        // Move the player while respecting collisions
        rb.MovePosition(targetPosition);
    }
}
