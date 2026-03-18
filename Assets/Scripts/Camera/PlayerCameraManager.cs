using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraManager : MonoBehaviour
{
	//Returns the forward direction of the camera for the player's movement
    public Vector3 CameraForward
    {
        get
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            return forward.normalized;
        }
    }

}