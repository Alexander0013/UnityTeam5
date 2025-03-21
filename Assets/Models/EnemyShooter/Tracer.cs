using UnityEngine;

public class Tracer : MonoBehaviour
{
    public float speed = 50f; // Adjust the speed of the tracer.

    private Vector3 target;
    private bool targetSet = false;

    public void SetTarget(Vector3 targetPosition)
    {
        target = targetPosition;
        targetSet = true;
    }

    private void Update()
    {
        if (!targetSet) return;

        // Move the tracer towards the target.
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
