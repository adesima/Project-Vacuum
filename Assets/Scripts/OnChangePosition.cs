using UnityEngine;

public class OnChangePosition : MonoBehaviour
{
    public PolygonCollider2D hole2DCollider;
    public PolygonCollider2D ground2DCollider;
    // public GameObject hole;
    public MeshCollider GeneratedMeshCollider;
    public float initialScale = 0.5f;
    Mesh GeneratedMesh;

    void FixedUpdate()
    {
        if (transform.hasChanged == true)
        {
            // Debug.Log("Position changed: " + hole.transform.position);
            transform.hasChanged = false;
            hole2DCollider.transform.position = new Vector2(transform.position.x, transform.position.z);
            hole2DCollider.transform.localScale = transform.localScale * initialScale;

            MakeHole2D();
            Make3DMeshCollider();
        }
    }

    void MakeHole2D()
    {
        Vector2[] PointPositions = hole2DCollider.GetPath(0);
        
        for (int i = 0; i < PointPositions.Length; i++)
        {
            PointPositions[i] += (Vector2)hole2DCollider.transform.TransformPoint(PointPositions[i]);
        }

        ground2DCollider.pathCount = 2;
        ground2DCollider.SetPath(1, PointPositions);
    }

    private void Make3DMeshCollider()
    {
        if (GeneratedMesh != null)
        {
            Destroy(GeneratedMesh);
        }
        
        GeneratedMesh = ground2DCollider.CreateMesh(true, true);
        GeneratedMeshCollider.sharedMesh = GeneratedMesh;
    }
}
