using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    [SerializeField]
    float cameraSpeed = 100f;
    [SerializeField]
    float moveDamping = 5f;


    [Header("Camera Zooming")]
    [SerializeField]
    float cameraSize = 40f;
    [SerializeField]
    float minZoom = 6f;
    [SerializeField]
    float maxZoom = 50f;
    [SerializeField]
    float zoomDamping = 5f;
    [SerializeField]
    float zoomSpeed = 2f;

    // Velocity
    Vector2 velocity = new Vector2();

    Vector2Int worldSize;
    float zoomVel;


    // Camera
    Camera cam;
    void Start(){
        cam = GetComponent<Camera>();
        MoveToCenter();
    }

    void MoveToCenter(){
        worldSize = new Vector2Int(FindAnyObjectByType<GenerateWorld>().worldSize.x, FindAnyObjectByType<GenerateWorld>().worldSize.y);
        transform.position = new Vector3(worldSize.x/2, worldSize.y/2, -10);
    }
    void Update()
    {
        // Checks if the pause menu isnt open
        if (!FindAnyObjectByType<PauseMenu>().open){
            moveCamera();
            transform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime;
            clampPosition();
            bool overUI = EventSystem.current.IsPointerOverGameObject();

            if (!overUI){
                zoomCamera();
            }  
        }
        
    }

    void clampPosition(){
        float tx = transform.position.x;
        float ty = transform.position.y;

        tx = Mathf.Clamp(tx, 0, worldSize.x);
        ty = Mathf.Clamp(ty, 0, worldSize.y);

        transform.position = new Vector3(tx, ty, -10);
    }

    void moveCamera(){
        InputAction move = InputSystem.actions.FindAction("Move");
        velocity += move.ReadValue<Vector2>() * (cameraSpeed * (cameraSize / maxZoom));

        // Deceleration
        velocity = Vector2.SmoothDamp(velocity, Vector2.zero, ref velocity, moveDamping * Time.deltaTime);

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
        zoomVel += zoomSpeed * Math.Sign(zoom.ReadValue<float>());
        // Clamps speed
        zoomVel = Mathf.Clamp(zoomVel, -zoomSpeed, zoomSpeed);
        // Deceleration
        zoomVel = Mathf.SmoothDamp(zoomVel, zoomVel * Math.Sign(zoom.ReadValue<float>()), ref zoomVel, zoomDamping * Time.deltaTime);
        // changes camera size by vel
        cameraSize += zoomVel;

        // Clamps camera size
        cameraSize = Mathf.Clamp(cameraSize, minZoom, maxZoom);

        cam.orthographicSize = cameraSize;
    }
}
