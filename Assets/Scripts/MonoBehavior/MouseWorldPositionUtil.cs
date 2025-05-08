using System;
using Unity.Assertions;
using UnityEngine;

public class MouseWorldPositionUtil : MonoBehaviour
{
    public static MouseWorldPositionUtil Instance { get; private set; }

    [SerializeField]
    private Camera mainCamera;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Assert.IsNotNull(mainCamera, "Camera not set!!");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = GetHitPosition();
            Debug.Log("Hit Pos : " + pos);
        }
    }

    public Vector3 GetHitPosition()
    {
        if (mainCamera == null)
        {
            return Vector3.zero;
        }
        
        Ray mouseCameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        Plane plane = new Plane(Vector3.up, Vector3.zero );
        if (plane.Raycast(mouseCameraRay, out float distance))
        {
            return mouseCameraRay.GetPoint(distance);
        }
        
        return Vector3.zero;
    }
}
