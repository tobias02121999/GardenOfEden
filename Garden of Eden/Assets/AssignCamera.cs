using UnityEngine;

public class AssignCamera : MonoBehaviour
{
    public NetworkPlayers players;
    public Camera localCentreEyeCamera;
    public Canvas canvas;

    private void Awake() 
    {
        localCentreEyeCamera = players.localPlayer.transform.Find("CenterEyeAnchor").GetComponent<Camera>();

        if (localCentreEyeCamera != null)
        {
            Debug.Log(localCentreEyeCamera.name);
            canvas.worldCamera = localCentreEyeCamera;
        }
    }
}
