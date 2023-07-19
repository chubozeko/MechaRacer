using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxSpeed = 15f;
    public float drag = 0.98f;
    public float steerAngle = 20f;
    public float traction = 1;
    public AudioSource car_audio;
    public AudioClip car_rev;
    public AudioClip car_hit;

    public GameObject[] wheels;

    private Vector3 moveForce;
    private Vector3 brakeForce;
    private float vertMove = 0f;
    private float horzMove = 0f;

    void Start()
    {
        car_audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Movement
        moveForce += transform.forward * moveSpeed * vertMove * Time.deltaTime;
        transform.position += moveForce * Time.deltaTime;

        // Steering
        float steerInput = horzMove;
        transform.Rotate(Vector3.up * steerInput * moveForce.magnitude * steerAngle * Time.deltaTime);

        // Drag and max speed limit
        moveForce *= drag;
        moveForce = Vector3.ClampMagnitude(moveForce, maxSpeed);

        // Traction
        Debug.DrawRay(transform.position, moveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.red);
        moveForce = Vector3.Lerp(moveForce.normalized, transform.forward, traction * Time.deltaTime) * moveForce.magnitude;

        vertMove = 0;
    }

    public void UpdateVerticalMovement(float vm)
    {
        vertMove += vm;
    }

    public void UpdateSteering(float hm)
    {
        horzMove = hm;
        // steerAngle = steerRotation;
        /*
        foreach (GameObject wheel in wheels)
        {
            wheel.transform.rotation = Quaternion.Euler(wheel.transform.rotation.x, wheel.transform.rotation.y + steerRotation, wheel.transform.rotation.z);
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            car_audio.PlayOneShot(car_hit);
            // Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            other.gameObject.GetComponent<AudioSource>().PlayOneShot(other.gameObject.GetComponent<AudioSource>().clip);
            Destroy(other.gameObject);
        }
    }
}
