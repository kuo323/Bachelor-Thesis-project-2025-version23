using UnityEngine;

public class LaserRayCast : MonoBehaviour
{
    public float range = 200f;

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Shield"))
            {
                hit.collider
                    .GetComponent<ShieldCollision>()
                    ?.OnLaserHit();
            }
        }
    }
}
