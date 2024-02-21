using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;

public class EnemyTurret : Enemy
{
    [SerializeField] float projectileFireRate;
    float timeSinceLastFire = 0;

    public Transform playerTransform;
    [SerializeField] float playerDetectDistance = 3.0f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (projectileFireRate <= 0)
        {
            projectileFireRate = 2.0f;
        }

        if (playerDetectDistance <= 0)
        {
            playerDetectDistance = 3.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Face player
        sr.flipX = playerTransform.position.x < transform.position.x;

        AnimatorClipInfo[] currentPlayingClips = anim.GetCurrentAnimatorClipInfo(0);

        if (currentPlayingClips[0].clip.name != "WitchShoot" && Vector3.Distance(playerTransform.position, this.transform.position) < playerDetectDistance)
        {
            // This method of rating projectile fire is simpler / more naive than coroutines
            if (Time.time >= timeSinceLastFire + projectileFireRate)
            {
                anim.SetTrigger("Fire");
                timeSinceLastFire = Time.time; // Time.time is the time since the game started
            }
        }
    }
}
