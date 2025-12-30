using UnityEngine;

public class ShieldCollision : MonoBehaviour
{


    public RotationGainController rotationGainController;

    public void OnLaserHit(LaserRayCast laser)
    {
        rotationGainController?.StartRedirection(laser);
    }

 

}
