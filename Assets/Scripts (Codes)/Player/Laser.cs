using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float laserSpeed = 1.0f;

    void Start()
    {
        
    }

    
    void Update()
    {
        transform.position += new Vector3(0, laserSpeed, 0) * Time.deltaTime;
    }
}
