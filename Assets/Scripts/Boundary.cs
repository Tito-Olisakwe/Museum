using UnityEngine;

public class Boundary : MonoBehaviour
{
    public Vector3 minBounds;
    public Vector3 maxBounds;

    private Transform xrOrigin;

    void Start()
    {
        xrOrigin = transform;
    }

    void Update()
    {
        Vector3 currentPosition = xrOrigin.position;

        xrOrigin.position = new Vector3(
            Mathf.Clamp(currentPosition.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(currentPosition.y, minBounds.y, maxBounds.y),
            Mathf.Clamp(currentPosition.z, minBounds.z, maxBounds.z)
        );
    }
}