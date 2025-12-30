using UnityEngine;

public class LaserRayCast : MonoBehaviour
{
    public float range = 200f;
    public GameObject laserBeam; // assign in Inspector or find dynamically


    [Header("Laser Audio (Looping)")]
    public AudioSource laserAudio;

    private bool laserDisabled = false;



    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Shield"))
            {
                hit.collider
                    .GetComponent<ShieldCollision>()
                    ?.OnLaserHit(this);
            }
        }
    }



    void Start()
    {

        // Ensure laser audio starts correctly
        if (laserAudio != null && !laserAudio.isPlaying)
        {
            laserAudio.loop = true;
            laserAudio.Play();
        }


    }

    public void DisableLaser()
    {
        if (laserDisabled) return;
        laserDisabled = true;

        // Disable laser visuals
        if (laserBeam != null)
            laserBeam.SetActive(false);

        // Stop looping laser audio
        if (laserAudio != null)
            laserAudio.Stop();

    }

}
