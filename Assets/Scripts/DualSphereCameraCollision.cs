using UnityEngine;
using Cinemachine;

public class DualSphereCameraCollision : CinemachineExtension
{
    [Header("Collision Settings")]
    // Layers to check against
    public LayerMask collisionLayers;

    // A larger radius for early collision detection.
    public float detectionRadius = 0.7f;

    // A smaller radius, roughly matching the camera collider size.
    public float colliderRadius = 0.5f;

    // Offset to prevent the camera from exactly touching the obstacle.
    public float collisionOffset = 0.2f;

    // Minimum distance allowed between the camera and the target.
    public float minDistance = 1f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        // We only adjust during the Body stage.
        if (stage != CinemachineCore.Stage.Body)
            return;

        // Ensure there is a valid target to look at.
        Transform target = vcam.LookAt;
        if (target == null)
            return;

        // Get the desired camera position from Cinemachine.
        Vector3 desiredCameraPos = state.RawPosition;
        Vector3 targetPos = target.position;

        // Determine the direction and distance from the target to the desired camera position.
        Vector3 direction = desiredCameraPos - targetPos;
        float desiredDistance = direction.magnitude;
        if (desiredDistance <= 0.01f)
            return;
        direction.Normalize();

        // First sphere cast with a larger radius for detection.
        if (Physics.SphereCast(targetPos, detectionRadius, direction, out RaycastHit detectionHit, desiredDistance, collisionLayers))
        {
            // Use the distance from the detection sphere cast as the maximum allowed distance.
            float maxAllowedDistance = detectionHit.distance;

            // Now do a second sphere cast with a smaller radius, more representative of the camera.
            if (Physics.SphereCast(targetPos, colliderRadius, direction, out RaycastHit colliderHit, maxAllowedDistance, collisionLayers))
            {
                float adjustedDistance = colliderHit.distance - collisionOffset;
                adjustedDistance = Mathf.Clamp(adjustedDistance, minDistance, desiredDistance);
                state.RawPosition = targetPos + direction * adjustedDistance;
            }
            else
            {
                // If the smaller sphere cast doesn't hit but the larger one did,
                // adjust based on the detection hit.
                float adjustedDistance = maxAllowedDistance - collisionOffset;
                adjustedDistance = Mathf.Clamp(adjustedDistance, minDistance, desiredDistance);
                state.RawPosition = targetPos + direction * adjustedDistance;
            }
        }
    }
}
