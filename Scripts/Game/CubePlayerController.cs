using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CubePlayerController : NetworkBehaviour
{

    [SerializeField]
    private GameObject localSideParent;

    [SerializeField]
    private float force = 100;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            localSideParent.SetActive(true);
        }else
        {
            Destroy(localSideParent);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputs.CubeInputData data))
        {
            Movement(data);
        }
    }

    
    private void Movement(NetworkInputs.CubeInputData cubeInputData)
    {
        Vector3 movement = new Vector3(cubeInputData.Horizontal, 0, cubeInputData.Vertical);
        _rigidbody.AddForce(movement * force * Runner.DeltaTime);
    }
}
