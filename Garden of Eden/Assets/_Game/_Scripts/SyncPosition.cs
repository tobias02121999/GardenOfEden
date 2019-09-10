using UnityEngine;
using UnityEngine.Networking;

public class SyncPosition : NetworkBehaviour
{
    [SyncVar] Vector3 syncPosition; // SyncVar syncs the variable to ALL clients (insane wtf)

    [SerializeField] Transform localTransform;
    [SerializeField] float smoothing = 15f;

    private void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            localTransform.position = Vector3.Lerp(localTransform.position, syncPosition, Time.deltaTime * smoothing);  // Smoothes the movement
        }
    }

    [Command]
    void CmdRelayPosition(Vector3 position) // Tells the server what position to lerp from and to.
    {
        syncPosition = position;    
    }

    [ClientCallback]
    void TransmitPosition() // Provides the local transform to relay to the server.
    {
        if (isLocalPlayer)
        {
            CmdRelayPosition(localTransform.position); 
        }
    }
}
