using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float cameraDistanceMax = 50f;
    private float cameraDistanceMin = 5f;
    private float cameraDistance = 30f;
    private float scrollSpeed = 7.5f;

    TurnManager TurnManager;
    // Start is called before the first frame update
    void Start()
    {
        TurnManager = GameObject.FindGameObjectWithTag("TurnManager").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!TurnManager.CreateActorMode)
        {
            MoveCamera();
            ZoomCamera();
        }   
    }

    private void ZoomCamera()
    {
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
        Vector3 newZoom = new Vector3(Camera.main.transform.localPosition.x, cameraDistance, Camera.main.transform.localPosition.z);
        Camera.main.transform.localPosition = newZoom;
    }

    private void MoveCamera()
    {
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (Input.GetKey(KeyCode.A))
        {
            newPos.x = transform.position.x + 0.1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPos.x = transform.position.x - 0.1f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            newPos.z = transform.position.z - 0.1f;

        }
        if (Input.GetKey(KeyCode.S))
        {
            newPos.z = transform.position.z + 0.1f;
        }

        Camera.main.transform.position = newPos;
    }
}
