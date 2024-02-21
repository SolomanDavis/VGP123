using System.Collections;
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
    // TODO: ZA - understand what [SerializeField actually does] is
    [SerializeField] float speed = 7.0f;
    [SerializeField] float jumpForce = 7.0f;

    // Ground check related fields
    [SerializeField] bool isGrounded = false;
    [SerializeField] Transform GroundCheck; // GroundCheck is a transform attached to the player used to check if the player is on the ground
    [SerializeField] LayerMask GroundLayer; // LayerMask holds the layer we will want to check within
    [SerializeField] float groundCheckRadius = 0.2f; // Radius of the circle used to check if the player is on the ground

    // Lives and score
    [SerializeField] int maxLives = 5;

    private int _lives;
    public int lives
    {
        get => _lives;
        set
        {
            if (value > maxLives)
            {
                value = 5;
            }
            else if (value < 0)
            {
                value = 0;
            }
            else
            {
                _lives = value;
            }

            Debug.Log("ZA - lives: " + _lives);
        }
    }

    private int _score;
    public int score
    {
        get => _score;
        set
        {
            // Input validation (can score decrease? etc.)
            _score = value;
            Debug.Log("ZA - Score: " + _score);
        }
    }

    // Coroutine
    Coroutine jumpForceChange = null;

    public void StartJumpForceChange()
    {
        // Doing this means that hitting a powerup near the end of the jump force change time span will not stack
        // But we can't just remove as we'll just be starting many coroutines.
        // Instead we should increase the timespan of the coroutine instead.
        if (jumpForceChange == null)
        {
            jumpForceChange = StartCoroutine(JumpForceChange());
            return;
        }

        // Stop existing coroutine and start a new one
        StopCoroutine(jumpForceChange);

        // Need to run cleanup code of coroutine - there should be a way to abstract this somehow
        jumpForceChange = null;
        jumpForce /= 2;

        jumpForceChange = StartCoroutine(JumpForceChange());
    }

    IEnumerator JumpForceChange()
    {
        jumpForce *= 2.0f;

        // Suspend coroutine
        yield return new WaitForSeconds(5.0f);

        jumpForce /= 2.0f;

        // Ensure only run once per coroutine
        jumpForceChange = null;
    }

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

        if (lives == 0)
        {
            lives = maxLives;
        }
    }

    // Update is called once per frame
    // 
    // Changes are applied in the PhysicsUpdate cycle. Is this cycle separate from the Update cycle? Is it concurrent?
    void Update()
    {
        // Determine if player is grounded
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, GroundLayer);

        // Reset gravity if player is grounded
        if (isGrounded)
        {
            rb.gravityScale = 1;
            anim.ResetTrigger("jumpAttackTrigger"); // Also reset trigger to reset unity buffered active trigger
        }

        // Move input
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
        if (xInput != 0) sr.flipX = xInput == -1;

        // Jump input
        // Use GetButtonDown instead of y axis input to ensure the jumpforce is applied correctly,
        // otherwise it will be applied multiple times
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // JumpAttack Input
        if (Input.GetButtonDown("Jump") && !isGrounded)
        {
            anim.SetTrigger("jumpAttackTrigger");
        }

        // GetCurrentAnimatorClipInfo returns array of all animation clips in a specific animation layer
        // The first element of the array is the currently playing animation clip
        //
        // Note: Animation layer is not the same as game object layer (animation layers more useful for 3D animations)
        AnimatorClipInfo[] clipInfos = anim.GetCurrentAnimatorClipInfo(0);
        AnimatorClipInfo currentClip = clipInfos[0];

        // Throw input
        //
        // Force player to be still on the x-axis when throwing
        if (currentClip.clip.name == "Throw")
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (Input.GetButtonDown("Throw"))
        {
            anim.SetTrigger("throwTrigger");
        }

        // Animation
        anim.SetFloat("xInput", Mathf.Abs(xInput));
        anim.SetFloat("yInput", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    // Public methods can be called by unity events - e.g. animation event
    //
    // Note: Need to also reset trigger as unity will queue the trigger event and the trigger
    // will remain active unless manually reset
    public void increaseGravity()
    {
        rb.gravityScale = 5;
    }

    // Collider events
    // Different physics bodies will trigger different events depending on what they are colliding with.

    // Trigger events are called most times but still generally require one physics body
    // Called on the frame entered on trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Squish"))
        {
            collision.transform.parent.gameObject.GetComponent<Enemy>().TakeDamage(999);

            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Called on the frame 2 onwards while in trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    // Called on the frame exited on trigger exit
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    // Collision events are only called when one of the two objects colliding is a dynamic rigidbody
    // Called on the frame entered on collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    // Called on the frame 2 onwards while in collision
    private void OnCollisionStay2D(Collision2D collision)
    {
    }

    // Called on the frame exited on collision exited
    private void OnCollisionExit2D(Collision2D collision)
    {
    }
}
