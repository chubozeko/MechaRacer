using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]

public class HandController : MonoBehaviour
{
    ActionBasedController controller;
    public Hand hand;
    private float gripValue;
    private float triggerValue;

    public GameObject nearbyObject;
    public GameObject grabbedObject;
    private bool isNearAGrabbable = false;
    private Vector3 controllerPos;
    private Quaternion controllerRot;

    void Start()
    {
        controller = GetComponent<ActionBasedController>();
    }

    void Update()
    {
        gripValue = controller.selectAction.action.ReadValue<float>();
        hand.SetGrip(gripValue);

        triggerValue = controller.activateAction.action.ReadValue<float>();
        hand.SetTrigger(triggerValue);

        if (gripValue > 0)
        {
            if (nearbyObject && isNearAGrabbable)
            {
                grabbedObject = nearbyObject;
                if (grabbedObject.GetComponent<SteeringWheel>() != null)
                {
                    SteeringWheel sWheel = grabbedObject.GetComponent<SteeringWheel>();
                    sWheel.EnableGrab(hand.transform, hand.tag);

                    if (sWheel.IsHeld())
                    {


                        /*
                        Quaternion rotationVec = transform.rotation * Quaternion.Inverse(controllerRot);
                        grabbedObject.transform.rotation = rotationVec * grabbedObject.transform.rotation;
                        Vector3 initVec = grabbedObject.transform.position - transform.position;
                        Vector3 turnedVec = rotationVec * initVec;
                        Vector3 attachVec = turnedVec - initVec;
                        Vector3 dPos = transform.position - controllerPos;
                        grabbedObject.transform.position += dPos + attachVec;
                        */
                    }
                }
            }
        }
        else
        {
            if (grabbedObject && grabbedObject.GetComponent<SteeringWheel>() != null)
            {
                grabbedObject.GetComponent<SteeringWheel>().DisableGrab(hand.transform, hand.tag);
                grabbedObject = null;
            }
        }

        
        if (grabbedObject && grabbedObject.GetComponent<SteeringWheel>() != null)
        {
            if (grabbedObject.GetComponent<SteeringWheel>().IsHeld())
            {
                if (triggerValue > 0)
                {
                    controller.activateAction.action.started += (ctx) =>
                    {
                        // grabbedObject.GetComponent<SteeringWheel>().car.car_audio.PlayOneShot(grabbedObject.GetComponent<SteeringWheel>().car.car_rev);
                    };

                    if (hand.gameObject.CompareTag("RightHand"))
                    {
                        grabbedObject.GetComponent<SteeringWheel>().car.UpdateVerticalMovement(triggerValue);
                        // grabbedObject.GetComponent<SteeringWheel>().car.GetComponent<AudioSource>().PlayOneShot(grabbedObject.GetComponent<SteeringWheel>().car.car_rev);
                        Debug.Log(hand.gameObject.name + " Trigger pressed: ACCELERATE! " + triggerValue);
                    }

                    if (hand.gameObject.CompareTag("LeftHand"))
                    {
                        grabbedObject.GetComponent<SteeringWheel>().car.UpdateVerticalMovement(-triggerValue);
                        Debug.Log(hand.gameObject.name + " Trigger pressed: REVERSE!" + (-triggerValue));
                    }
                } else
                {
                    grabbedObject.GetComponent<SteeringWheel>().car.UpdateVerticalMovement(triggerValue);
                }

                
            }
        }

        

        controllerPos = transform.position;
        controllerRot = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grabbedObject == null)
        {
            if (other.CompareTag("Grabbable"))
            {
                isNearAGrabbable = true;
                nearbyObject = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (grabbedObject == null)
        {
            if (other.CompareTag("Grabbable"))
            {
                isNearAGrabbable = false;
                nearbyObject = null;
            }
        }
    }
}
