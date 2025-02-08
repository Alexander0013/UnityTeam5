using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The object to focus on
    public float rotationSpeed = 5f; // Speed of rotation
    public float zoomSpeed = 10f; // Speed of zooming
    public float moveSpeed = 5f; // Speed of movement
    public float verticalMoveSpeed = 3f; // Speed of vertical movement (up/down)
    public float minZoom = 2f; // Minimum zoom distance
    public float maxZoom = 20f; // Maximum zoom distance

    private float distance = 10f;
    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        if (target != null)
        {
            distance = Vector3.Distance(transform.position, target.position);
        }
    }

    void Update()
    {
        // Zoom in/out using Mouse ScrollWheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        // Rotate when holding right mouse button
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, -30f, 60f); // Limit vertical rotation
        }

        // Move left/right/forward/backward with arrow keys or WASD
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
        transform.position += transform.right * moveDirection.x + transform.forward * moveDirection.z;

        // Move up/down with Q/E keys
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.up * verticalMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position -= Vector3.up * verticalMoveSpeed * Time.deltaTime;
        }

        // Update camera position and rotation
        if (target != null)
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 offset = rotation * new Vector3(0, 0, -distance);
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}
