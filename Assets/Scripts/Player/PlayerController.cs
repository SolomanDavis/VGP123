using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public bool debug = true;

    // Component
    Rigidbody2D rb;
    SpriteRenderer sr;

    // Use SerializeField provided by Unity instead of using public field
    // Also default speed
    // TODO: ZA - understand what this is
    [SerializeField] float speed = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Default speed in case it is set to 0 in Unity
        if (speed <= 0)
        {
            speed = 7.0f;
            if (debug) Debug.Log("Default speed set to 7.0f");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Set automatically as range bw -1 and 1
        // This is what actually gets the input from the player (A [-1], D [1])
        float xInput = Input.GetAxis("Horizontal");
        // float yInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }
}
