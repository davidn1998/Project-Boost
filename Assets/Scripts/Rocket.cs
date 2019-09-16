using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    //configuration params
    [SerializeField] float mainThrust = 15f;
    [SerializeField] float rotThrust = 100f;

    //Cache Variables
    Rigidbody rb = null;
    AudioSource soundEffectPlayer = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        soundEffectPlayer = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();   
    }

    private void FixedUpdate()
    {
        Thrust();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Friendly"))
        {
            Debug.Log("Friendly object");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Thrust()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            if (!soundEffectPlayer.isPlaying)
            {
                soundEffectPlayer.Play();
            }
            rb.AddRelativeForce(Vector3.up * mainThrust);
        }
        else
        {
            soundEffectPlayer.Stop();
        }

    }

    private void Rotate()
    {
        rb.freezeRotation = true; //take manual control of rotation

        transform.Rotate(Input.GetAxis("Horizontal") * -Vector3.forward * Time.deltaTime * rotThrust);

        rb.freezeRotation = false; //resume physics control of rotation
    }
}
