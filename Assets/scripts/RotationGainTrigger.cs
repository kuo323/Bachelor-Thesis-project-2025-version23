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
         //   gainController.StartRedirection();

            distractionManager.SpawnCluster();
            distractionManager2.PickNewState();
            clusterSpawnedOnce = true;

            Debug.Log("➡ User entered trigger zone: rotation gain started & cluster spawned once.");
        }

    }


}
