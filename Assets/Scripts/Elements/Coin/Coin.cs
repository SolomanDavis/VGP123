using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Coin : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get incoming collider
        GameObject incoming = collision.gameObject;

        // If the incoming collider is the player
        if (incoming.CompareTag("Player"))
        {
            // Play the coin animation
            anim.SetTrigger("destroyCoinTrigger");
        }
    }

    public void OnAnimationDestroy()
    {
        Destroy(this.gameObject);
    }
}
