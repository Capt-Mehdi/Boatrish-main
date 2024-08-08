using System.Collections.Generic;
using UnityEngine;

public class DrawLine3D : MonoBehaviour
{
    public GameObject linePrefab;
    public PhysicMaterial zeroFrictionMaterial; // PhysicMaterial with zero friction
    public AudioClip drawSound;  // Sound to play when a line is drawn
    public AudioSource audioSource; // AudioSource component for playing sounds
    private LineRenderer currentLineRenderer;
    private List<Vector3> points;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }

        if (Input.GetMouseButton(0) && currentLineRenderer != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("River") || hit.collider.gameObject.GetComponent<Terrain>())
                {
                    Vector3 point = hit.point;
                    UpdateLine(point);
                }
            }
        }
    }

    void CreateLine()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("River") || hit.collider.gameObject.GetComponent<Terrain>())
            {
                GameObject line = Instantiate(linePrefab);
                line.tag = "Line";
                currentLineRenderer = line.GetComponent<LineRenderer>();

                if (currentLineRenderer == null)
                {
                    Debug.LogError("LinePrefab does not contain a LineRenderer component.");
                    return;
                }

                Rigidbody rb = line.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;

                points = new List<Vector3>();

                Vector3 point = hit.point;
                points.Add(point);
                points.Add(point);
                currentLineRenderer.positionCount = 2;
                currentLineRenderer.SetPosition(0, points[0]);
                currentLineRenderer.SetPosition(1, points[1]);

                // Play sound effect
                PlaySound(drawSound);

                // Apply zero friction material
                Collider lineCollider = line.GetComponent<Collider>();
                if (lineCollider != null && zeroFrictionMaterial != null)
                {
                    lineCollider.material = zeroFrictionMaterial;
                }
            }
        }
    }

    void UpdateLine(Vector3 newPoint)
    {
        if (points != null && currentLineRenderer != null && Vector3.Distance(newPoint, points[points.Count - 1]) > 0.1f)
        {
            points.Add(newPoint);
            currentLineRenderer.positionCount = points.Count;
            currentLineRenderer.SetPosition(points.Count - 1, newPoint);
            UpdateColliders();
        }
    }

    void UpdateColliders()
    {
        if (currentLineRenderer == null) return;

        foreach (Transform child in currentLineRenderer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 startPoint = points[i];
            Vector3 endPoint = points[i + 1];
            CreateBoxCollider(startPoint, endPoint);
        }
    }

    void CreateBoxCollider(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject colliderObject = new GameObject("LineCollider");
        colliderObject.transform.parent = currentLineRenderer.transform;
        BoxCollider boxCollider = colliderObject.AddComponent<BoxCollider>();

        Vector3 midPoint = (startPoint + endPoint) / 2;
        colliderObject.transform.position = midPoint;

        float length = Vector3.Distance(startPoint, endPoint);
        boxCollider.size = new Vector3(0.1f, 0.1f, length);

        Vector3 direction = endPoint - startPoint;
        colliderObject.transform.rotation = Quaternion.LookRotation(direction);

        // Apply zero friction material to the collider
        if (zeroFrictionMaterial != null)
        {
            boxCollider.material = zeroFrictionMaterial;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
