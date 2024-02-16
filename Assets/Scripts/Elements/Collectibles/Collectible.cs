using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Collectible : MonoBehaviour
{
    Animator anim;

    public enum PickupType
    {
        Score,
        Life,
        Powerup,
        Key
    }
    public PickupType pickupType;

    public float timeToDestroy = 1.0f;

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
        if (collision.CompareTag("Player"))
        {
            anim.SetTrigger("destroyTrigger");

            PlayerController pc = collision.GetComponent<PlayerController>();
            switch (pickupType)
            {
                case PickupType.Score:
                    ++pc.score;
                    break;
                case PickupType.Life:
                    ++pc.lives;
                    break;
                case PickupType.Powerup:
                    pc.StartJumpForceChange();
                    break;
                case PickupType.Key:
                    break;
            }
        }
    }

    public void OnAnimationDestroy()
    {
        Destroy(this.gameObject);
    }
}
