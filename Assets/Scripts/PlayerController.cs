using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public static bool dead = false;         // For the "dying animation"
    private bool ranOver = false;            // To make sure OntriggerEnter is called only once
    private bool audioPlaying = false;       // To make sure the audio clip is played only once
    private bool rotationDone = false;      
    private bool reachedLeft = false;
    private bool reachedRight = false;
    private int leftLimit = -9;
    private int rightLimit = 9;
    private float fatMeter = 1;
    private Vector3 move;
    private Vector3 behindPosition;
    private ThirdPersonCharacter character; // A reference to the ThirdPersonCharacter on the object
    private CapsuleCollider capsule;
    private AudioSource audioPlayer;
    [SerializeField]
    private AudioClip deathSound; 

    private void Start()
    {
        // Get the collider
        capsule = GetComponent<CapsuleCollider>();
        // Make sure it is a trigger collider
        capsule.isTrigger = true;

        // get the third person character ( this should never be null due to require component )
        character = GetComponent<ThirdPersonCharacter>();

        // get the audio source
        audioPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!dead)
        {
            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !reachedLeft)
                move = Vector3.left;
            else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !reachedRight)
                move = Vector3.right;
            else
                move = Vector3.zero;
            reachedLeft = transform.position.x < leftLimit;
            reachedRight = transform.position.x > rightLimit;
            // pass all parameters to the character control script
            character.Move(move, false, false);
        }
        // "Death animation"
        //----------------------------------------------------------------------------------------------------------------------------------------
        else if (!rotationDone)
        {
            character.Move(Vector3.zero, false, false); //Stop the character's movement jumping and crouching
            StartCoroutine(FinishRotation(1)); //Terminate the rotation process in case the end rotation is not perfect
            //Rotation process
            //------------------------------------------------------------------------------------------------------
            Quaternion facingDirection = Quaternion.LookRotation((behindPosition - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, facingDirection, 0.1f);
            if (transform.rotation == facingDirection)
                rotationDone = true;
            //------------------------------------------------------------------------------------------------------

        }
        else if (fatMeter < 2f)
        {
            if(!audioPlaying)
            {
                audioPlaying = true;
                audioPlayer.clip = deathSound;
                audioPlayer.Play();
            }
            fatMeter += Time.deltaTime;
            //This makes the model wider to simulate him getting fat because he got run over by a food item
            transform.Find("EthanSkeleton").Find("EthanHips").Find("EthanSpine").Find("EthanSpine1").localScale = new Vector3(1, 1, fatMeter);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------
        else
        {
            dead = false;
            GameManager.LoadScores();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy" && !ranOver)
        {
            dead = true;
            behindPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            GameObject.Find("Spawn Manager").SetActive(false);
            GameObject.Find("Food Items").SetActive(false);
            ranOver = true;
        }
    }

    private IEnumerator FinishRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        rotationDone = true;
    }
}
