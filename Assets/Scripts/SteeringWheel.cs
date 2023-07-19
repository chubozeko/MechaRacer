using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    public CarController car;
    public float maxSteerAngleLeft = -60f;
    public float maxSteerAngleRight = 60f;
    public bool canSteer = false;
    private bool isHeld = false;
    public Transform leftGrabber;
    public Transform rightGrabber;
    public AudioSource steeringWheelAudio;
    public AudioClip steeringWheelHold;
    private bool isGrabbedByLeft = false;
    private bool isGrabbedByRight = false;
    private float lastRotY;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();  
        steeringWheelAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (canSteer)
        {
            // Vector3 euler = transform.localRotation.eulerAngles;
            // euler.y = Mathf.Clamp(euler.y, maxSteerAngleLeft, maxSteerAngleRight);
            // transform.localRotation = Quaternion.Euler(euler);
            
            if (transform.localRotation.y <= maxSteerAngleLeft)
            {
                transform.localRotation = Quaternion.Euler(transform.rotation.x, maxSteerAngleLeft, transform.rotation.z).normalized;
            }
            else if (transform.localRotation.y >= maxSteerAngleRight)
            {
                transform.localRotation = Quaternion.Euler(transform.rotation.x, maxSteerAngleRight, transform.rotation.z).normalized;
            }

            if (transform.localRotation.y < lastRotY)
                car.UpdateSteering(-1f);
            else
                car.UpdateSteering(1f);

            lastRotY = transform.localRotation.y;
        }
    }

    public void EnableGrab(Transform grabber, string hand)
    {
        if (hand.Equals("LeftHand"))
        {
            leftGrabber = grabber;
            isGrabbedByLeft = true;
        }
        else
        {
            rightGrabber = grabber;
            isGrabbedByRight = true;
        }

        if (IsHeld())
        {
            rb.isKinematic = false;
            canSteer = true;
        }

        // steeringWheelAudio.PlayOneShot(steeringWheelHold);
    }

    public void DisableGrab(Transform grabber, string hand)
    {
        if (hand.Equals("LeftHand"))
        {
            leftGrabber = null;
            isGrabbedByLeft = false;
        }
        else
        {
            rightGrabber = null;
            isGrabbedByRight = false;
        }

        if (!IsHeld())
        {
            rb.isKinematic = true;
            canSteer = false;
        }
    }

    public bool IsHeld()
    {
        return isGrabbedByLeft && isGrabbedByRight;
    }
}
