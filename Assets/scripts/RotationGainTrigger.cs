using UnityEngine;

public class RotationGainTrigger : MonoBehaviour
{
    public RotationGainController gainController;
    public DistractionManager distractionManager;
    public DistractionManager2 distractionManager2;

    public bool clusterSpawnedOnce { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {


        if (!other.CompareTag("MainCamera")) return;

        if (!gainController.isRedirecting && !clusterSpawnedOnce)
        {
         

          //  distractionManager.SpawnCluster();

            // Spawn 1 meter in front of the player's head
            Vector3 spawnPos = CameraManager.Instance.head.position + CameraManager.Instance.head.forward * 1f;
            distractionManager2.SpawnBot(spawnPos);

            clusterSpawnedOnce = true;

            Debug.Log("➡ User entered trigger zone: rotation gain started & cluster spawned once.");
        }

    }


}
