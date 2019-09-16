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

    //Cache Variables
    Rigidbody rb = null;
    AudioSource audioSource = null;
    Vector3 startPosition;
    enum State {Alive, Dying, Transcending};
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
        startPosition = transform.position;
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
        audioSource.Stop();
        thrusterParticles.Stop();

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
        audioSource.PlayOneShot(deathSFX);
        Instantiate(deathParticles.gameObject, transform.position, Quaternion.identity);
        state = State.Dying;
        Invoke("Respawn", respawnDelay);
    }

    //Loads scene 2
    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    //Respawns the player
    private void Respawn()
    {
        state = State.Alive;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Produces the thrust force for the rocket when "jump" axis is input
    private void Thrust()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(thrusterSFX);
            }
            thrusterParticles.Play();
            rb.AddRelativeForce(Vector3.up * mainThrust);
        }
        else
        {
            audioSource.Stop();
            thrusterParticles.Stop();
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
