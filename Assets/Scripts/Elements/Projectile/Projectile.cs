using Unity.VisualScripting;
using UnityEngine;

// ZA - Remember:
// Always use Prefabs as a template to create objects that are meant to be created
// dynamically. Static objects can simply be placed in the scene (if they're unique).
// Otherwise Prefabs are the way to go.

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))]
public class Projectile : MonoBehaviour
{
    public float lifeTime; // Life time of projectile in seconds
    public Vector2 initialVelocity; // Initial velocity of projectile

    Rigidbody2D rb;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (lifeTime <= 0)
        {
            lifeTime = 2.0f;
        }

        // Set initial velocity for the first frame and let unity gravity take care of the rest
        //
        // If we wanted to change gravity behavior we could make some edits to the RigidBody in
        // the unity editor. We can do it programmatically in Update() if we want something more manual.
        rb.velocity = initialVelocity;

        // Destroy the projectile after lifeTime seconds
        Destroy(gameObject, lifeTime);
    }

    // Collider events
    // Different physics bodies will trigger different events depending on what they are colliding with.

    // Trigger events are called most times but still generally require one physics body
    // Called on the frame entered on trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
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
        // Get incoming collider
        GameObject incoming = collision.gameObject;

        switch (incoming.tag)
        {
            case "Ground":
                // Determine collision direction by checking contact point normal
                ContactPoint2D contact = collision.GetContact(0);
                Debug.DrawRay(contact.point, contact.normal, Color.red);

                // If collision occurs horizontally freeze the projectile and destroy it
                if (contact.normal.Abs().x >= contact.normal.Abs().y)
                {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0;
                    anim.SetTrigger("destroyTrigger");
                }

                break;

            case "Coin":
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                anim.SetTrigger("destroyTrigger");
                break;

            case "Enemy":
                if (this.tag == "EnemyProjectile")
                {
                    Destroy(this.gameObject);
                }

                collision.gameObject.GetComponent<Enemy>().TakeDamage(1);
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                anim.SetTrigger("destroyTrigger");
                break;

            case "Player":
                if (this.tag == "PlayerProjectile")
                {
                    Destroy(this.gameObject);
                }

                collision.gameObject.GetComponent<PlayerController>().lives--;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                anim.SetTrigger("destroyTrigger");
                break;
        }
    }

    // Called on the frame 2 onwards while in collision
    private void OnCollisionStay2D(Collision2D collision)
    {
    }

    // Called on the frame exited on collision exited
    private void OnCollisionExit2D(Collision2D collision)
    {
    }

    public void OnAnimationDestroy()
    {
        Destroy(this.gameObject);
    }
}
