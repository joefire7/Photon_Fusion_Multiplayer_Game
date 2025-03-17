using System;
using Fusion;
using UnityEngine;

[Serializable]
public struct CarInputData : INetworkInput
{
    public Vector3 Direction;
    public bool IsBraking;
    public bool IsRocketing;
    public bool IsJumping;
    public bool IsShooting;
    public bool IsForceToCenter;
}