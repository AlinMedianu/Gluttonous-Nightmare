using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The parent class for every food movement type behaviour 
public abstract class FoodMovement : MonoBehaviour
{
    [SerializeField]
    protected float foodSpeed = 2000;
    protected Rigidbody rb;

    protected float FoodSpeed
    {
        get
        {
            return foodSpeed;
        }
        set
        {
            if(name == "Chicken(Clone)" || name == "Fries(Clone)")
                foodSpeed = value;
            else
                foodSpeed = value * 0.5f;
        }
    }
    protected virtual void Start()
    {
        //Adapting each item's speed with the difficulty increase
        FoodSpeed = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>().FoodSpeed;
        rb = GetComponent<Rigidbody>();
    }
    //The extra movement
    protected abstract void FixedUpdate();
}
