using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigZagMovement : FoodMovement
{
    [SerializeField]
    private float leftLimit = -10f;
    [SerializeField]
    private float rightLimit = 10f;

    //goes from left to right in a zigzag pattern
    protected override void FixedUpdate()
    {
		if(rb.position.x < leftLimit)
            rb.velocity = (Vector3.back + Vector3.right).normalized * Time.deltaTime * FoodSpeed;
        else if(rb.position.x > rightLimit)
            rb.velocity = (Vector3.back + Vector3.left).normalized * Time.deltaTime * FoodSpeed;
    }
}
