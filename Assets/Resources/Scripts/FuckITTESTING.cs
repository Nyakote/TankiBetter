using UnityEngine;
using System;
using System.Collections.Generic;

public class FuckITTESTING : MonoBehaviour
{
    public List<WheelCollider> wheelsRight = new List<WheelCollider>();
    public List<WheelCollider> wheelsLeft = new List<WheelCollider>();

    public float powerR = 10f;
    public float powerL = -10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void FixedUpdate()
    {
        foreach (WheelCollider wheel in wheelsLeft)
        {
            wheel.motorTorque = powerL;
        }

        foreach (WheelCollider wheel in wheelsRight)
        {
            wheel.motorTorque = powerR;
        }
    }
}
