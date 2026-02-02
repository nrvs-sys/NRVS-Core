using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PID 
{
    public static Vector3 CalculateMovementForce(Vector3 currentPosition, Vector3 targetPosition, Vector3 currentVelocity, Vector3 targetVelocity, float positionFrequency, float positionDamping, float deltaTime)
    {
        float kp = (6f * positionFrequency) * (6f * positionFrequency) * 0.25f;
        float kd = 4.5F * positionFrequency * positionDamping;
        float g = 1 / (1 + kd * deltaTime + kp * deltaTime * deltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * deltaTime) * g;
        return (targetPosition - currentPosition) * ksg + (targetVelocity - currentVelocity) * kdg;
    }

    public static Vector3 CalculateRotationTorque(Quaternion currentRotation, Quaternion targetRotation, Vector3 currentAngularVelocity, float rotationFrequency, float rotationDamping, float deltaTime)
    {
        float kp = (6f * rotationFrequency) * (6f * rotationFrequency) * 0.25f;
        float kd = 4.5f * rotationFrequency * rotationDamping;
        float g = 1 / (1 + kd * deltaTime + kp * deltaTime * deltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * deltaTime) * g;
        Quaternion q = targetRotation * Quaternion.Inverse(currentRotation);
        if (q.w < 0)
        {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        return ksg * axis * angle + -currentAngularVelocity * kdg;
    }

    public static void HookesLaw(Vector3 currentPosition, Vector3 targetPosition, Vector3 currentVelocity, float climbForce, float climbDrag, float deltaTime, out Vector3 acceleration, out Vector3 drag)
    {
        Vector3 displacementFromResting = currentPosition - targetPosition;
        acceleration = displacementFromResting * climbForce;
        drag = GetDrag(currentPosition, currentVelocity, deltaTime) * -currentVelocity * climbDrag;
    }

    static float GetDrag(Vector3 currentPosition, Vector3 lastPosition, float deltaTime)
    {
        Vector3 handvelocity = (currentPosition - lastPosition) / deltaTime;
        float drag = 1 / handvelocity.magnitude + 0.01f;
        drag = drag > 1 ? 1 : drag;
        drag = drag < 0.03f ? 0.03f : drag;
        return drag;
    }

}
