using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorL;
    [SerializeField] private GameObject doorR;
    [SerializeField] private GameObject doorLtarget;
    [SerializeField] private GameObject doorRtarget;

    public float openTime;

    [SerializeField] private GameObject activator;
    [SerializeField] private GameObject activator2;

    private Vector3 doorLstartPos;
    private Vector3 doorRstartPos;

    private void Start()
    {
        doorLstartPos = doorL.transform.position;
        doorRstartPos = doorR.transform.position;
    }

    private void Update()
    {
        MoveDoor();
    }

    private void MoveDoor() 
    {
        if (activator.gameObject.GetComponent<DoorActivate>().isActivated || activator2.gameObject.GetComponent<DoorActivate>().isActivated) 
        {
            doorL.transform.position = Vector3.MoveTowards(doorL.transform.position, doorLtarget.transform.position, openTime);
            doorR.transform.position = Vector3.MoveTowards(doorR.transform.position, doorRtarget.transform.position, openTime);
        }
        else 
        {
            doorL.transform.position = Vector3.MoveTowards(doorL.transform.position, doorLstartPos, openTime);
            doorR.transform.position = Vector3.MoveTowards(doorR.transform.position, doorRstartPos, openTime);
        }
    }
}
