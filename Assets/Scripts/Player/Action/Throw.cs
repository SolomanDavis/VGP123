using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Throw : MonoBehaviour
{
    SpriteRenderer sr;

    public Vector2 initialVelocity;
    public Transform spawnPointRight;
    public Transform spawnPointLeft;
    public Projectile projectilePreFab;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Shoot here is a public method so that it can be called on an animation event
    // In unity prefer to trigger methods and functions using animation events so that the timings
    // are all in sync.
    // In this case, should hook up this method on throw animation start.
    public void Shoot()
    {
        if (!sr.flipX)
        {
            Projectile currentProjectile = Instantiate(projectilePreFab, spawnPointRight.position, spawnPointRight.rotation);
            currentProjectile.initialVelocity = initialVelocity;
        }
        else
        {
            Projectile currentProjectile = Instantiate(projectilePreFab, spawnPointLeft.position, spawnPointLeft.rotation);
            currentProjectile.initialVelocity = new Vector2(-initialVelocity.x, initialVelocity.y);
        }
    }
}
