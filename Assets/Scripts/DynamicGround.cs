using UnityEngine;

public class DynamicGround : MonoBehaviour
{
    [Header("Referințe")]
    [Tooltip("Trage aici cilindrul tău (Gaura)")]
    public Transform hole;
    
    [Header("Setări Teren (Ground)")]
    [Tooltip("Cât de mare este pământul tău pe axa X")]
    public float groundWidth = 20f; 
    [Tooltip("Cât de mare este pământul tău pe axa Z")]
    public float groundLength = 20f; 
    public float groundThickness = 1f; // Grosimea pământului
    
    [Header("Setări Gaură")]
    public float holeRadius = 1f; // Raza găurii (jumătate din diametrul cilindrului)

    // Cele 4 colidere invizibile care vor forma pământul
    private BoxCollider topCol, bottomCol, leftCol, rightCol;

    void Start()
    {
        // Generăm coliderele automat când începe jocul
        topCol = gameObject.AddComponent<BoxCollider>();
        bottomCol = gameObject.AddComponent<BoxCollider>();
        leftCol = gameObject.AddComponent<BoxCollider>();
        rightCol = gameObject.AddComponent<BoxCollider>();
    }

    void Update()
    {
        // Poziția curentă a găurii
        Vector3 hPos = hole.position;
        
        // Calculăm jumătățile terenului pentru a-l centra la (0,0,0)
        float halfW = groundWidth / 2f;
        float halfL = groundLength / 2f;
        
        // Pământul va fi așezat exact sub coordonata Y = 0
        float yPos = -groundThickness / 2f;

        // 1. Coliderul de SUS (Top) - Acoperă spațiul de deasupra găurii
        float topZSize = halfL - (hPos.z + holeRadius);
        topCol.size = new Vector3(groundWidth, groundThickness, topZSize);
        topCol.center = new Vector3(0, yPos, (halfL + hPos.z + holeRadius) / 2f);

        // 2. Coliderul de JOS (Bottom) - Acoperă spațiul de sub gaură
        float bottomZSize = (hPos.z - holeRadius) - (-halfL);
        bottomCol.size = new Vector3(groundWidth, groundThickness, bottomZSize);
        bottomCol.center = new Vector3(0, yPos, (-halfL + hPos.z - holeRadius) / 2f);

        // 3. Coliderul din STÂNGA (Left) - Se aliniază în stânga găurii
        float leftXSize = (hPos.x - holeRadius) - (-halfW);
        leftCol.size = new Vector3(leftXSize, groundThickness, holeRadius * 2);
        leftCol.center = new Vector3((-halfW + hPos.x - holeRadius) / 2f, yPos, hPos.z);

        // 4. Coliderul din DREAPTA (Right) - Se aliniază în dreapta găurii
        float rightXSize = halfW - (hPos.x + holeRadius);
        rightCol.size = new Vector3(rightXSize, groundThickness, holeRadius * 2);
        rightCol.center = new Vector3((halfW + hPos.x + holeRadius) / 2f, yPos, hPos.z);
    }
}