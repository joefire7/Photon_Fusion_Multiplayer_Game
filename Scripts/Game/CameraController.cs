using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using EditorButton;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera ballCamera;

    [SerializeField]
    private CinemachineVirtualCamera carCamera;

    [SerializeField]
    private CinemachineVirtualCamera dynamicCamera;

    private bool _isCarCameraActice;

    private void Start()
    {
        _isCarCameraActice = true;
        UpdateCameraStates();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCamera();
        }
    }

    private void UpdateCameraStates()
    {
        if (_isCarCameraActice)
        {
            carCamera.Priority = 1;
            ballCamera.Priority = 0;
        }else
        {
            carCamera.Priority = 0;
            ballCamera.Priority = 1;
        }
    }

    [Button]
    private void ToggleCamera()
    {
        _isCarCameraActice = !_isCarCameraActice;
        UpdateCameraStates();
    }

    public void SetGameBall(Transform ballVisual)
    {
        ballCamera.LookAt = ballVisual;
    }

    // Dynamic Cam when the player aproach the Enemy
    public void EnemyDynamicCamera(bool statusCam)
    {
        bool dynamic = false;
        dynamic = statusCam;

        if (dynamic)
        {
            carCamera.Priority = 0;
            ballCamera.Priority = 0;
            dynamicCamera.Priority = 1;
        }else if (!dynamic)
        {
            carCamera.Priority = 1;
            ballCamera.Priority = 0;
            dynamicCamera.Priority = 0;
        }
    }
}
