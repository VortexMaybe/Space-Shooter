using UnityEngine;

public class Enemy_Tank : EnemyBase
{
    [Header("Tank Specifics")]
    [SerializeField] float tankSpeed = 1.5f;

   

   
    protected override void Start()
    {
        base.Start();
   
    }

    protected override void Update()
    {
        
        transform.Translate(Vector3.down * tankSpeed * Time.deltaTime);

    
        base.Update();
    }
}