using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    //configuration params
    [SerializeField] float mainThrust = 15f;
    [SerializeField] float rotThrust = 100f;
    [SerializeField] float respawnDelay = 1f;

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

    private void FixedUpdate()
    {

        if (state == State.Alive)
        {
            Thrust();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!(state == State.Alive)) { return; }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            state = State.Dying;
            audioSource.Stop();
            Invoke("Die", respawnDelay);
        }
        else if(collision.gameObject.CompareTag("Finish"))
        {
            state = State.Transcending;
            Invoke("LoadNextScene", respawnDelay);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void Die()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        state = State.Alive;
    }

    private void Thrust()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            rb.AddRelativeForce(Vector3.up * mainThrust);
        }
        else
        {
            audioSource.Stop();
        }

    }

    private void Rotate()
    {
        rb.freezeRotation = true; //take manual control of rotation

        transform.Rotate(Input.GetAxis("Horizontal") * -Vector3.forward * Time.deltaTime * rotThrust);

        rb.freezeRotation = false; //resume physics control of rotation
    }
}
