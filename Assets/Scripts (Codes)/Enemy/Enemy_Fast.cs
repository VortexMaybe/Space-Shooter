using UnityEngine;
using System.Collections;

public class Enemy_Fast : EnemyBase
{
    [Header("Fast Specifics")]
    [SerializeField] float fastSpeed = 7f;
    [SerializeField] float wiggleMagnitude = 1.5f;

    private float initialX;

    protected override void Start()
    {
        base.Start();
        initialX = transform.position.x; 
        StartCoroutine(ShootRoutine(3.5f, null)); 
    }

    protected override void Update()
    {
       
        float sinXOffset = Mathf.Sin(Time.time * 5f) * wiggleMagnitude;
        Vector3 newPosition = transform.position + Vector3.down * fastSpeed * Time.deltaTime;

        
        transform.position = new Vector3(initialX + sinXOffset, newPosition.y, 0);

        base.Update();
    }

   
    protected IEnumerator ShootRoutine(float interval, GameObject laserPrefab)
    {
        yield break;
    }
}