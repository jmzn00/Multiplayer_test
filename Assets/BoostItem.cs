using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoostItem : MonoBehaviourPun
{
    private Rigidbody playerRigidbody;
    public float forceAmount = 10f;
    private bool shouldBoost;
    Item itemScript;
    float cooldown;
    float timer = 0f;

    private void Start()
    {
        if (!photonView.IsMine) { return; }
        playerRigidbody = transform.root.GetComponent<Rigidbody>();
        itemScript = GetComponent<Item>();
        cooldown = GetComponent<Item>().cooldown;
        UI_Controller.instance.itemUseSlider.maxValue = cooldown;
    }
    
    private void Update()
    {
        if (!photonView.IsMine) { return; }
        if (Input.GetMouseButtonDown(1) && itemScript._itemAmount > 0 && timer >= cooldown)
        {            
            shouldBoost = true;
            timer = 0f;
        }
        timer += Time.deltaTime;
        UI_Controller.instance.itemUseSlider.value = timer;
        //Debug.Log(timer);
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine) { return; }
        if (shouldBoost) 
        {
            playerRigidbody.AddForce(playerRigidbody.transform.forward * forceAmount, ForceMode.Impulse);
            shouldBoost = false;
        }
    }
}
