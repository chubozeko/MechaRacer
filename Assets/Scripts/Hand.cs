using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Hand : MonoBehaviour
{
    // Animation
    [SerializeField] private float animationSpeed = 10f;
    private Animator animator;
    // private SkinnedMeshRenderer mesh;
    private float gripTarget;
    private float triggerTarget;
    private float gripCurrent;
    private float triggerCurrent;
    private string animGripParam = "Grip";
    private string animTriggerParam = "Trigger";

    // Physics Movement
    [SerializeField] private GameObject followObject;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    private Transform followTarget;
    private Rigidbody rb;

    void Start()
    {
        // Animation
        animator = GetComponent<Animator>();
        // mesh = GetComponentInChildren<SkinnedMeshRenderer>();

        // Physics Movement
        followTarget = followObject.transform;
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 20f;

        // Teleport hands
        rb.position = followTarget.position;
        rb.rotation = followTarget.rotation;
    }

    void Update()
    {
        AnimateHand();

        PhysicsMove();
    }

    internal void SetGrip(float v)
    {
        gripTarget = v;
    }

    internal void SetTrigger(float v)
    {
        triggerTarget = v;
    }

    private void AnimateHand()
    {
        if (gripCurrent != gripTarget)
        {
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * animationSpeed);
            animator.SetFloat(animGripParam, gripCurrent);
        }

        if (triggerCurrent != triggerTarget)
        {
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * animationSpeed);
            animator.SetFloat(animTriggerParam, triggerCurrent);
        }
    }

    private void PhysicsMove()
    {
        // Position
        var positionWithOffset = followTarget.position + positionOffset;
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        rb.velocity = (positionWithOffset - transform.position).normalized * (followSpeed * distance);

        // Rotation
        var rotationWithOffset = followTarget.rotation * Quaternion.Euler(rotationOffset);
        var quat = rotationWithOffset * Quaternion.Inverse(rb.rotation);
        quat.ToAngleAxis(out float angle, out Vector3 axis);
        rb.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
    }
}
