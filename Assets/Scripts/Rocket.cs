using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    //configuration params
    [Header("Gameplay")]
    [SerializeField] float respawnDelay = 1f;

    [Header("Movement")]
    [SerializeField] float mainThrust = 15f;
    [SerializeField] float rotThrust = 100f;

    [Header("Sound")]
    [SerializeField] AudioClip thrusterSFX = null;
    [SerializeField] AudioClip deathSFX = null;
    [SerializeField] AudioClip winSFX = null;

    [Header("Particle Effects")]
    [SerializeField] ParticleSystem thrusterParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] Transform thrusterParticlePos;

    //Cache Variables
    Rigidbody rb = null;
    AudioSource audioSource = null;
    Vector3 startPosition;
    enum State {Alive, Dying, Transcending};
    State state = State.Alive;
    int currentSceneIndex;
    ParticleSystem thrusterParticlesObj;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
        startPosition = transform.position;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive) {
            Rotate();
        }
    }

    //For rigidbody physics updates
    private void FixedUpdate()
    {

        if (state == State.Alive)
        {
            Thrust();
        }
    }

    //Checks collision with other objects.
    private void OnCollisionEnter(Collision collision)
    {
        if (!(state == State.Alive)) { return; }

        ProcessCollision(collision);
    }

    //Processes collision depending on tag
    private void ProcessCollision(Collision collision)
    {
        StopThrusters();

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            StartDeathSequence();
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            StartWinSequence();
        }
    }

    //Win the level
    private void StartWinSequence()
    {
        audioSource.PlayOneShot(winSFX);
        Instantiate(winParticles.gameObject, transform.position, Quaternion.identity);
        state = State.Transcending;
        Invoke("LoadNextScene", respawnDelay);
    }

    //Die on the level
    private void StartDeathSequence()
    {
        audioSource.PlayOneShot(deathSFX, 0.5f);
        Instantiate(deathParticles.gameObject, transform.position, Quaternion.identity);
        state = State.Dying;
        Invoke("Respawn", respawnDelay);
    }

    //Loads scene 2
    private void LoadNextScene()
    {
        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentSceneIndex);
;       }
        else
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    //Respawns the player
    private void Respawn()
    {
        state = State.Alive;
        SceneManager.LoadScene(currentSceneIndex);
    }

    //Starts and stops thrusters depending on input
    private void Thrust()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            StartThrusters();
        }
        else
        {
            StopThrusters();
        }

    }

    //Start thrusters - instantiate particles, play audio and move rocket
    private void StartThrusters()
    {
        //audio
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(thrusterSFX);
        }

        //particles
        if (!thrusterParticlesObj)
        {
            thrusterParticlesObj = Instantiate(thrusterParticles, thrusterParticlePos.position, Quaternion.identity, transform);
        }

        //physics
        rb.AddRelativeForce(Vector3.up * mainThrust);
    }

    //Stop thrusters - stop audio and destoy particles 
    private void StopThrusters()
    {
        //audio
        audioSource.Stop();

        //particles
        if (thrusterParticlesObj)
        {
            Destroy(thrusterParticlesObj.gameObject);
        }
    }

    //Rotates the rocket based on horizontal axis input
    private void Rotate()
    {
        rb.freezeRotation = true; //take manual control of rotation

        transform.Rotate(Input.GetAxis("Horizontal") * -Vector3.forward * Time.deltaTime * rotThrust);

        rb.freezeRotation = false; //resume physics control of rotation
    }
}
