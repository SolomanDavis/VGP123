using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowPlayer : MonoBehaviour
{
    Camera cm;

    public Transform PlayerTransform;

    [SerializeField] float offsetY = 0.0f;

    // Bounding colliders of the camera
    public Transform LeftBound;
    public Transform RightBound;
    public Transform BottomBound;

    private float minX, maxX, minY;

    // Velocity used for smooth dampening of camera following
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        cm = GetComponent<Camera>();

        if (LeftBound == null || RightBound == null || BottomBound == null)
        {
            Debug.LogError("Camera bounds not set correctly");
            return;
        }

         // Calculate the camera bounds based on the BoxCollider positions and the camera's viewable width
        float cameraHalfWidth = cm.orthographicSize * ((float) Screen.width / Screen.height);
        float cameraHalfHeight = cm.orthographicSize;
        minX = LeftBound.transform.position.x + cameraHalfWidth;
        maxX = RightBound.transform.position.x - cameraHalfWidth;
        minY = BottomBound.transform.position.y + cameraHalfHeight;
    }

    // FixedUpdate is called on a fixed interval
    void FixedUpdate()
    {
        Vector3 cameraPos = PlayerTransform.transform.position + new Vector3(0, offsetY, -5);

        // Clamp the camera position to within the bounds
        cameraPos.x = Mathf.Clamp(cameraPos.x, minX, maxX);
        cameraPos.y = Mathf.Clamp(cameraPos.y, minY, Mathf.Infinity);

        transform.position = Vector3.SmoothDamp(transform.position, cameraPos, ref velocity, 0.1f);
    }
}
