using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    public float cameraSpeed = 100f;
    public float moveDamping = 5f;


    [Header("Camera Zooming")]
    public float cameraSize = 40f;
    public float minZoom = 6f;
    public float maxZoom = 50f;
    public float zoomDamping = 5f;
    public float zoomSpeed = 2f;

    // Velocity
    Vector2 velocity = new Vector2();
    float zoomVel;

    // Camera
    Camera cam;
    void Start(){
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        moveCamera();
        transform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime;
        zoomCamera();
    }

    void moveCamera(){
        InputAction move = InputSystem.actions.FindAction("Move");
        velocity += move.ReadValue<Vector2>() * cameraSpeed;

        // Deceleration
        velocity = Vector2.Lerp(velocity, Vector2.zero, moveDamping * Time.deltaTime);

        clampVelocity();
    }

    void clampVelocity(){
        if (velocity.magnitude > cameraSpeed){
            velocity = velocity.normalized * cameraSpeed;
        }
    }

    void zoomCamera(){
        InputAction zoom = InputSystem.actions.FindAction("Zoom");
        // Adds zoom speed to vel
        zoomVel += zoomSpeed * Mathf.Round(zoom.ReadValue<float>());
        // Clamps speed
        zoomVel = Mathf.Clamp(zoomVel, -zoomSpeed, zoomSpeed);
        // Deceleration
        zoomVel = Mathf.Lerp(zoomVel, zoomVel * Mathf.Round(zoom.ReadValue<float>()), zoomDamping * Time.deltaTime);
        // changes camera size by vel
        cameraSize += zoomVel;

        // Clamps camera size
        cameraSize = Mathf.Clamp(cameraSize, minZoom, maxZoom);

        cam.orthographicSize = cameraSize;
    }
}
