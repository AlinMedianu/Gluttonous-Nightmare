using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMovement : FoodMovement
{
    [SerializeField]
    private float homingLimit = 20f;
    private bool homing = false;

    //Stops in front of the player, alows him to get out of the way 
    //and, then, goes towards the location the player was in the moment this item stopped
    protected override void FixedUpdate()
    {
        if (rb.position.z < homingLimit && !homing)
        {
            StartCoroutine(Follow(1));
            homing = true;
        }
	}
    private IEnumerator Follow(float seconds)
    {
        Vector3 direction = (GameObject.Find("Player").transform.position - rb.position).normalized;
        //Give the player time to react to the incoming train that is this food item
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(seconds);
        rb.velocity = direction * Time.deltaTime * FoodSpeed;
    }
}
