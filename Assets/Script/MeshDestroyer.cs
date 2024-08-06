using UnityEngine;

public class MeshDestroyer : MonoBehaviour
{
    public float explosionForce = -12f; // Reduced explosion force
    public float explosionRadius = -0.8f; // Reduced explosion radius
    public int numberOfChunks = 2; // Number of chunks to break into

    private BoatMovement boatMovement;

    private void Start()
    {
        boatMovement = GetComponent<BoatMovement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleObstacleCollision();
        }
    }

    private void HandleObstacleCollision()
    {
        if (boatMovement != null)
        {
            boatMovement.HandleObstacleCollision();
        }
        DestroyMesh();
    }

    public void DestroyMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        int chunkSize = Mathf.CeilToInt((float)triangles.Length / (numberOfChunks * 3)) * 3;

        for (int i = 0; i < triangles.Length; i += chunkSize)
        {
            int length = Mathf.Min(chunkSize, triangles.Length - i);
            Vector3[] newVertices = new Vector3[length];

            for (int j = 0; j < length; j++)
            {
                newVertices[j] = vertices[triangles[i + j]];
            }

            CreateMeshPiece(newVertices);
        }

        // Optionally, you can hide the boat instead of destroying it immediately
        gameObject.SetActive(false);
    }

    private void CreateMeshPiece(Vector3[] vertices)
    {
        GameObject piece = new GameObject("Piece");
        piece.transform.position = transform.position;
        piece.transform.rotation = transform.rotation;
        piece.AddComponent<MeshFilter>().mesh = CreateMesh(vertices);
        piece.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        piece.AddComponent<BoxCollider>();
        Rigidbody rb = piece.AddComponent<Rigidbody>();
        
        // Add a random direction to the force to simulate a collision
        Vector3 randomDirection = (Random.insideUnitSphere + Vector3.up) * explosionForce;
        rb.AddForce(randomDirection, ForceMode.Impulse);

        Destroy(piece, 5f);
    }

    private Mesh CreateMesh(Vector3[] vertices)
    {
        Mesh mesh = new Mesh();
        int[] triangles = new int[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
