using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public abstract class Enemy : MonoBehaviour
{
    protected Animator anim;
    protected SpriteRenderer sr;

    protected int health;
    public int maxHealth;

    // Start is called before the first frame update
    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (maxHealth <= 0)
        {
            maxHealth = 10;
        }

        health = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            anim.SetTrigger("Death");
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
