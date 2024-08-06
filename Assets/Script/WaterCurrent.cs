using UnityEngine;

public class WaterCurrent : MonoBehaviour
{
    public Vector3 currentDirection = new Vector3(1, 0, 1); // Direction of the water flow
    public float currentStrength = 1f; // Strength of the water flow

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            BoatMovement boatMovement = other.GetComponent<BoatMovement>();
            if (boatMovement != null)
            {
                boatMovement.SetCurrentDirection(currentDirection.normalized);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            Rigidbody boatRigidbody = other.GetComponent<Rigidbody>();
            if (boatRigidbody != null)
            {
                // Apply the water current force to the boat continuously
                boatRigidbody.AddForce(currentDirection.normalized * currentStrength, ForceMode.Acceleration);
            }
        }
    }
}
