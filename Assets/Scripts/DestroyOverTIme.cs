using UnityEngine;

public class DestroyOverTIme : MonoBehaviour
{

    public float lifeTime = 1.5f;
        
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    
    void Update()
    {
        
    }
}
