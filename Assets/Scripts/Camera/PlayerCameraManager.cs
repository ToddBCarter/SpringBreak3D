using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraManager : MonoBehaviour
{	
/*
    private const float YMin = -85.0f;
    private const float YMax = 85.0f;

    public Transform lookAt;

    public Transform Player;

    public float distance = 10.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    public float sensivity = 1.0f;

*/

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

/*
    // Update is called once per frame
    void LateUpdate()
    {
        currentX += Mouse.current.delta.x.ReadValue() * sensivity;
        currentY += -(Mouse.current.delta.y.ReadValue() * sensivity);

        currentY = Mathf.Clamp(currentY, YMin, YMax);

        Vector3 Direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = lookAt.position + rotation * Direction;

        transform.LookAt(lookAt.position);

    }
	
	*/
}