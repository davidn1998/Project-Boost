using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

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
        Rotation();   
    }

    private void FixedUpdate()
    {
        Thrusters();
    }

    private void Thrusters()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            if (!soundEffectPlayer.isPlaying)
            {
                soundEffectPlayer.Play();
            }
            rb.AddRelativeForce(Vector3.up * 20);
        }
        else
        {
            soundEffectPlayer.Stop();
        }

    }

    private void Rotation()
    {
        transform.Rotate(Input.GetAxis("Horizontal") * -Vector3.forward * Time.deltaTime * 50);
    }
}
