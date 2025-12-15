using UnityEngine;

public class ShieldCollision : MonoBehaviour
{


    public RotationGainController rotationGainController;

    public void OnLaserHit()
    {
        rotationGainController?.StartRedirection();
    }
}
