namespace I2.Loc.Models.Input
{
    using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;

    void Start()
    {
        // Cache references for performance
        Rigidbody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // Assumes only one main camera is in the scene
    }

    void Update()
    {
        // Handle mouse input
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            StartDrag();
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            EndDrag();
        }

        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartDrag(touch.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndDrag();
                    break;
            }
        }

        if (isDragging)
        {
            Vector3 currentPosition;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            currentPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition) + offset;
#else
            currentPosition = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position) + offset;
#endif
            currentPosition.z = 0; // Ensure we're not moving the object in Z-axis
            Rigidbody2D.MovePosition(currentPosition);
        }
    }

    private void StartDrag(Vector3 inputPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
        worldPosition.z = 0;
        if (Vector2.Distance(Rigidbody2D.position, worldPosition) < 1f) // Example threshold for starting drag
        {
            isDragging = true;
            offset = transform.position - worldPosition;
        }
    }

    private void StartDrag()
    {
        StartDrag(Input.mousePosition);
    }

    private void EndDrag()
    {
        isDragging = false;
    }
}
}