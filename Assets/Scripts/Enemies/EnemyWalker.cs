using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class EnemyWalker : Enemy
{
    Rigidbody2D rb;

    [SerializeField] float xVelocity = 3.0f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        if (xVelocity <= 0)
        {
            xVelocity = 3.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] currentPlayingClips = anim.GetCurrentAnimatorClipInfo(0);

        if (currentPlayingClips[0].clip.name == "SkeletonWalk")
        {
            rb.velocity = sr.flipX ? new Vector2(xVelocity, rb.velocity.y) : new Vector2(-xVelocity, rb.velocity.y);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (damage == 999)
        {
            anim.SetTrigger("Squish");
            Destroy(gameObject.transform.parent.gameObject, 2.0f); // Destroy the parent object to destroy all siblings as well
            return;
        }

        base.TakeDamage(damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Skeleton1Bound"))
        {
            sr.flipX = !sr.flipX;
        }
    }
}
