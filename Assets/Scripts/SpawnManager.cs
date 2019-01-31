using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] foodItemPrefabs;

    [SerializeField]
    private GameObject spawnParticle;


    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private float spawnRate = 0.5f;

    [SerializeField]
    private float randomRange = 0.1f;

    [SerializeField]
    private float foodSpeed = 2000f;

    [SerializeField]
    private float foodRotationSpeed = 100f;

    private float timer;
    private float spawnTimer;
    private Vector3 previousSpawn;

    public float FoodSpeed
    {
        get
        {
            return foodSpeed;
        }
        set
        {
            foodSpeed = value;
        }
    }

    private void Start ()
    {
        spawnTimer = spawnRate;
    }

    private void Update ()
    {
        if (spawnTimer >= 0)
            spawnTimer -= Time.deltaTime;
        timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (spawnTimer < 0)
        {
            Vector3 spawnPos = GetSpawn();

            Instantiate(spawnParticle, spawnPos, Quaternion.identity);

            Transform foodItem = Instantiate(foodItemPrefabs[Random.Range(0, foodItemPrefabs.Length)], spawnPos, Quaternion.identity, GameObject.Find("Food Items").transform).transform;
            //The way the difficulty increases
            foodSpeed = Mathf.Pow(1.047f, timer * 2f) * 2f + 1998f;
            //Initial behaviours (continued by their own behaviours)
            //-----------------------------------------------------------------------------------------------------------------------
            if (foodItem.name == "Burger(Clone)" || foodItem.name == "Egg(Clone)" || foodItem.name == "Pizza(Clone)")
            {
                foodItem.GetComponent<Rigidbody>().velocity = (Vector3.back + Vector3.left).normalized * Time.deltaTime * foodSpeed;
                foodItem.GetComponent<Rigidbody>().AddTorque(Vector3.up * foodRotationSpeed, ForceMode.VelocityChange);
            }
            else if(foodItem.name == "Fries(Clone)")
            {
                foodItem.GetComponent<Rigidbody>().velocity = Vector3.back * Time.deltaTime * foodSpeed;
                foodItem.GetComponent<Rigidbody>().AddTorque(Vector3.up * foodRotationSpeed, ForceMode.VelocityChange);
            }
            else
            {
                foodItem.GetComponent<Rigidbody>().velocity = Vector3.back * Time.deltaTime * foodSpeed;
                foodItem.GetComponent<Rigidbody>().AddTorque(Vector3.left * foodRotationSpeed, ForceMode.VelocityChange);
            }
            //-----------------------------------------------------------------------------------------------------------------------
            spawnTimer = Random.Range(spawnRate - randomRange, spawnRate + randomRange);
            previousSpawn = spawnPos;
        }
    }

    private Vector3 GetSpawn()
    {
        Vector3 spawnPos;
        do
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        } while (spawnPos == previousSpawn);
        return spawnPos;
    }
}
