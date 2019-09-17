using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{

    //configuration params
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;

    //cache variables
    Vector3 startingPos;
    float movementFactor;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Oscillate();
    }

    private void Oscillate()
    {
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period; //grows continuall from 0;

        const float TAU = Mathf.PI * 2; //about 6.28
        float rawSinWave = Mathf.Sin(cycles * TAU); //from -1 to +1

        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
