using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Refrence to the virtual camera
    

    public float shakeDuration = 0.3f;
    public float shakeIntensity = 2.0f;
    public float shakeFrequency = 2.0f;

    private CinemachineTransposer transposer;
    // Start is called before the first frame update
    void Start()
    {
        if(virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        }
    }

    public void ShakeCamera()
    {
        if(transposer != null)
        {
            // Generate random noise for shake effect
            Vector3 noise = Random.insideUnitSphere * shakeIntensity;

            // Apply noise to the cameras follow offeset to simulate shake
            transposer.m_FollowOffset += noise;

            Invoke("StopShaking", shakeDuration);
        }
    }

    private void StopShaking()
    {
        if(transposer != null)
        {
            Vector3 resetOffeset = new Vector3(1.95f, 1.85f, -3.18f);
            transposer.m_FollowOffset = resetOffeset; // reset aplitude to stop shaking
        }
    }
}
