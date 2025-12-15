using UnityEngine;

public class ShieldController : MonoBehaviour
{

    public GameObject shield;

    private void Update()
    {
        bool rightTrigger = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);


        shield.SetActive(rightTrigger);


    }



}
