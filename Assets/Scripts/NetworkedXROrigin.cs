using Fusion;
using UnityEngine;

public class NetworkedXROrigin : NetworkBehaviour
{
    public Transform xrRigTransform;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority)
        {
            xrRigTransform.position = transform.position;
            xrRigTransform.rotation = transform.rotation;
        }
    }
}
