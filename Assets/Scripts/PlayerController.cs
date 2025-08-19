using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables to be tweaked in the editor
    public float moveSpeed = 5f;
    public float dashForce = 20f;

    // Private components
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Get the Rigidbody2D component on start
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Read WASD input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // Check for Dash input
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (moveInput != Vector2.zero)
            {
                rb.AddForce(moveInput * dashForce, ForceMode2D.Impulse);
            }
        }
    }

    // Called at a fixed interval, good for physics
    void FixedUpdate()
    {
        // Apply movement to the rigidbody
        rb.velocity = moveInput * moveSpeed;
    }
}
