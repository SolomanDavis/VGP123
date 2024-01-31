using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public bool debug = true;

    // Component
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    // Use SerializeField provided by Unity instead of using public field
    // Also default speed
    // TODO: ZA - understand what this is
    [SerializeField] float speed = 7.0f;
    [SerializeField] float jumpForce = 7.0f;

    // Ground check related fields
    [SerializeField] bool isGrounded = false;
    [SerializeField] Transform GroundCheck; // GroundCheck is a transform attached to the player used to check if the player is on the ground
    [SerializeField] LayerMask GroundLayer; // LayerMask holds the layer we will want to check within
    [SerializeField] float groundCheckRadius = 0.2f; // Radius of the circle used to check if the player is on the ground

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Default speed in case it is incorrectly set
        if (speed <= 0)
        {
            speed = 7.0f;
            if (debug) Debug.Log("[" + gameObject.name + "] Default speed set to 7.0f");
        }

        // Default speed in case it is incorrectly set
        if (jumpForce <= 0)
        {
            jumpForce = 7.0f;
            if (debug) Debug.Log("[" + gameObject.name + "] Default jumpForce set to 7.0f");
        }

        // Default GroundCheck in case it is not set
        if (GroundCheck == null)
        {
            // Try to find a game object tagged GroundCheck first
            GameObject taggedGroundCheck = GameObject.FindGameObjectWithTag("GroundCheck");
            if (taggedGroundCheck != null) GroundCheck = taggedGroundCheck.transform;

            // Otherwise create and attach a GroundCheck
            if (GroundCheck == null)
            {
                GameObject obj = new GameObject("GroundCheck-Generated");
                obj.tag = "GroundCheck"; // Always remember to tag and apply the appropriate layers when creating game objects programmatically
                obj.layer = LayerMask.NameToLayer("Ground");
                obj.transform.SetParent(gameObject.transform);
                obj.transform.localPosition = Vector2.zero;
                GroundCheck = obj.transform;
                if (debug) Debug.Log("[" + gameObject.name + "] GroundCheck not set, created a new one and attached to the player");
            }
        }

        if (groundCheckRadius <= 0)
        {
            groundCheckRadius = 0.2f;
            if (debug) Debug.Log("[" + gameObject.name + "] Default groundCheckRadius set to 0.2f");
        }

        if (GroundLayer == 0)
        {
            GroundLayer = LayerMask.GetMask("Ground");
            if (debug) Debug.Log("[" + gameObject.name + "] GroundLayer not set, default to Ground layer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Determine if player is grounded
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, GroundLayer);

        // Move input
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
        if (xInput != 0) sr.flipX = xInput == -1;

        // Jump input
        // Use GetButtonDown instead of y axis input to ensure the jumpforce is applied correctly,
        // otherwise it will be applied multiple times
        if (Input.GetButtonDown("Jump") && isGrounded) rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Animation
        anim.SetFloat("xInput", Mathf.Abs(xInput));
        anim.SetFloat("yInput", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        if (Input.GetButtonDown("Throw")) anim.SetTrigger("throwTrigger");
    }
}
