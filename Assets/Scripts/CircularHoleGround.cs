using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CircularHoleGround : MonoBehaviour
{
    [Header("Referințe")]
    [Tooltip("Trage aici cilindrul tău (Gaura)")]
    public Transform hole;

    [Header("Setări Gaură și Teren")]
    [Tooltip("Raza găurii (jumătate din scala cilindrului)")]
    public float holeRadius = 1f;
    [Tooltip("Cât de detaliat să fie cercul (32 e perfect)")]
    public int segments = 32;
    [Tooltip("Trebuie să fie mult mai mare decât harta ta vizuală")]
    public float giantGroundSize = 100f; 

    private MeshCollider meshCollider;
    private Mesh generatedMesh;

    void Start()
    {
        // Setăm Rigidbody-ul pentru a nu fi afectat de gravitație, ci doar să poată fi mutat de noi
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        rb.useGravity = false;

        meshCollider = GetComponent<MeshCollider>();
        GenerateCircularHoleMesh();
    }

    void GenerateCircularHoleMesh()
    {
        generatedMesh = new Mesh();
        generatedMesh.name = "GiantPhysicsGround";

        // Vom avea un cerc interior (gaura) și un cerc uriaș exterior (pământul)
        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];

        float angleStep = 360f / segments;

        // Calculăm punctele (vârfurile)
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            // Punctele pentru gaura interioară
            vertices[i] = new Vector3(x * holeRadius, 0, z * holeRadius);
            
            // Punctele pentru marginea uriașă a pământului invizibil
            vertices[i + segments] = new Vector3(x * giantGroundSize, 0, z * giantGroundSize);
        }

        // Unim punctele pentru a crea suprafața (Triunghiurile)
        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            int inner1 = i;
            int inner2 = next;
            int outer1 = i + segments;
            int outer2 = next + segments;

            // Triunghiul 1
            triangles[ti++] = inner1;
            triangles[ti++] = inner2;
            triangles[ti++] = outer1;

            // Triunghiul 2
            triangles[ti++] = inner2;
            triangles[ti++] = outer2;
            triangles[ti++] = outer1;
        }

        generatedMesh.vertices = vertices;
        generatedMesh.triangles = triangles;
        generatedMesh.RecalculateNormals(); // Pentru ca fizica să știe care e "sus" și "jos"

        // Aplicăm forma creată pe colider
        meshCollider.sharedMesh = generatedMesh;
    }

    void FixedUpdate()
    {
        // Mutăm pământul invizibil odată cu gaura, pe axele X și Z
        if (hole != null)
        {
            transform.position = new Vector3(hole.position.x, transform.position.y, hole.position.z);
        }
    }
}