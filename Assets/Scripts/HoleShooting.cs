// using UnityEngine;
// using UnityEngine.InputSystem; // Necesar pentru noul sistem de input (Mouse)

// public class HoleShooting : MonoBehaviour
// {
//     [Header("Verificare Nivel")]
//     public HoleFeeding stomachScript; // Referință către stomac pentru a citi 'isBossLevel'
//     public HoleInventory inventoryScript; // Referință către inventar pentru muniție

//     [Header("Elemente Vizuale")]
//     public LineRenderer lineRenderer;
//     public GameObject targetIndicator; // Cercul de pe pământ (un prefab sau obiect plat)

//     [Header("Setări Catapultă")]
//     [Tooltip("Punctul de unde zboară obiectul (ex: gura găurii)")]
//     public Transform shootPoint; 
//     [Tooltip("Unghiul de lansare în sus (în grade, ex: 45)")]
//     public float launchAngle = 45f;
//     [Tooltip("Forța maximă de aruncare")]
//     public float maxForce = 20f;

//     [Header("Setări Traiectorie (Parabolă)")]
//     public int resolution = 30; // Câte puncte are linia
//     public float timeStep = 0.05f; // Distanța în timp dintre puncte

//     private bool isAiming = false;
//     private Vector3 currentVelocity;

//     void Start()
//     {
//         // Ascundem linia și cercul la început
//         if (lineRenderer != null) lineRenderer.enabled = false;
//         if (targetIndicator != null) targetIndicator.SetActive(false);
//     }

//     void Update()
//     {
//         // ATENȚIE: Dacă NU este boss level, oprim complet scriptul aici
//         if (stomachScript == null || !stomachScript.isBossLevel)
//         {
//             CancelAiming();
//             return;
//         }

//         // Dacă nu avem muniție, nu putem ținti
//         if (!inventoryScript.HasAmmo())
//         {
//             CancelAiming();
//             return;
//         }

//         HandleShootingInput();
//     }

//     void HandleShootingInput()
//     {
//         // 1. Apăsare Click Stânga - Începem țintirea
//         if (Mouse.current.leftButton.wasPressedThisFrame)
//         {
//             isAiming = true;
//             if (lineRenderer != null) lineRenderer.enabled = true;
//             if (targetIndicator != null) targetIndicator.SetActive(true);
//         }

//         // 2. Menținere Click Stânga - Actualizăm traiectoria în timp real
//         if (isAiming && Mouse.current.leftButton.isPressed)
//         {
//             CalculateLaunchVelocity();
//             DrawTrajectory();
//         }

//         // 3. Eliberare Click Stânga - Tragem!
//         if (isAiming && Mouse.current.leftButton.wasReleasedThisFrame)
//         {
//             FireProjectile();
//         }
//     }

//     void CalculateLaunchVelocity()
//     {
//         // Facem un Raycast de la cameră la poziția mouse-ului pe ecran pentru a găsi punctul pe pământ
//         Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
//         if (Physics.Raycast(ray, out RaycastHit hit))
//         {
//             Vector3 targetPoint = hit.point;
//             Vector3 fromPoint = shootPoint.position;

//             // Calculăm direcția pe orizontală (X și Z)
//             Vector3 direction = targetPoint - fromPoint;
//             direction.y = 0; // Ignorăm înălțimea inițială
            
//             float distance = direction.magnitude;
//             if (distance == 0) return;

//             // Calculăm forța în funcție de distanță, dar o limităm la maxForce
//             float force = Mathf.Min(distance * 1.5f, maxForce);

//             // Combinăm direcția orizontală cu unghiul în sus pentru a crea o parabolă perfectă
//             Vector3 baseVelocity = direction.normalized * Mathf.Cos(launchAngle * Mathf.Deg2Rad);
//             baseVelocity.y = Mathf.Sin(launchAngle * Mathf.Deg2Rad);

//             currentVelocity = baseVelocity * force;
//         }
//     }

//     void DrawTrajectory()
//     {
//         if (lineRenderer == null) return;

//         lineRenderer.positionCount = resolution;
//         Vector3 startPosition = shootPoint.position;
//         Vector3 lastPoint = startPosition;

//         bool hitGround = false;

//         for (int i = 0; i < resolution; i++)
//         {
//             float t = i * timeStep;
            
//             // Formula fizică a mișcării pe parabolă (Poziție = Start + Viteză*t + 0.5*Gravitate*t^2)
//             Vector3 point = startPosition + currentVelocity * t + 0.5f * Physics.gravity * t * t;

//             // Verificăm dacă parabola a lovit pământul (oprim linia acolo)
//             if (!hitGround && i > 0)
//             {
//                 if (Physics.Linecast(lastPoint, point, out RaycastHit hit))
//                 {
//                     // Dacă lovim ceva, setăm restul punctelor din linie în acel punct de impact
//                     hitGround = true;
//                     point = hit.point;
                    
//                     // Mutăm cercul de țintire exact unde cade bomba
//                     if (targetIndicator != null)
//                     {
//                         targetIndicator.transform.position = hit.point + new Vector3(0, 0.02f, 0); // Ușor deasupra solului
//                     }
//                 }
//             }

//             lineRenderer.SetPosition(i, point);
//             lastPoint = point;
//         }

//         // Dacă parabola zboară în infinit și nu atinge solul în raza vizuală, ascundem cercul temporar
//         if (!hitGround && targetIndicator != null)
//         {
//             targetIndicator.SetActive(false);
//         }
//         else if (hitGround && targetIndicator != null)
//         {
//             targetIndicator.SetActive(true);
//         }
//     }

//     // void FireProjectile()
//     // {
//     //     // Consumăm un proiectil din inventar (returnează prefab-ul salvat)
//     //     GameObject prefabToSpawn = inventoryScript.ConsumeAmmo();

//     //     if (prefabToSpawn != null)
//     //         {
//     //         // Instanțiem proiectilul la gura găurii
//     //         GameObject projectile = Instantiate(prefabToSpawn, shootPoint.position, Quaternion.identity);
            
//     //         // Îi dăm forța fizică calculată din parabolă
//     //         Rigidbody rb = projectile.GetComponent<Rigidbody>();
//     //         if (rb != null)
//     //         {
//     //             rb.isKinematic = false; // Ne asigurăm că fizica e pornită
//     //             rb.useGravity = true;
//     //             rb.AddForce(currentVelocity, ForceMode.VelocityChange);
//     //         }
//     //     }

//     //     CancelAiming();
//     // }

//     void FireProjectile()
//     {
//         // Luăm obiectul ascuns din inventar (cub, sferă, etc.)
//         GameObject projectile = inventoryScript.ConsumeAmmo();

//         if (projectile != null)
//         {
//             // 1. Îl mutăm la punctul de tragere
//             projectile.transform.position = shootPoint.position;
            
//             // 2. Îl reaprindem (îl facem vizibil)
//             projectile.SetActive(true);

//             // --- COD NOU: ÎNARMĂM PROIECTILUL ---
//             // Ne asigurăm că adăugăm scriptul de impact doar dacă nu îl are deja
//             if (projectile.GetComponent<ProjectileImpact>() == null)
//             {
//                 projectile.AddComponent<ProjectileImpact>();
//             }
//             // ------------------------------------
            
//             Rigidbody rb = projectile.GetComponent<Rigidbody>();
//             if (rb != null)
//             {
//                 // 3. FOARTE IMPORTANT: Resetăm viteza! 
//                 // Când l-ai mâncat, el cădea. Dacă nu resetăm viteza, va trage ciudat.
//                 rb.linearVelocity = Vector3.zero;
//                 rb.angularVelocity = Vector3.zero; 

//                 // 4. Îi aplicăm forța nouă calculată de parabolă
//                 rb.isKinematic = false; 
//                 rb.useGravity = true;
//                 rb.AddForce(currentVelocity, ForceMode.VelocityChange);
//             }
//         }

//         CancelAiming();
//     }

//     void CancelAiming()
//     {
//         isAiming = false;
//         if (lineRenderer != null) lineRenderer.enabled = false;
//         if (targetIndicator != null) targetIndicator.SetActive(false);
//     }
// }

using UnityEngine;
using UnityEngine.InputSystem;

public class HoleShooting : MonoBehaviour
{
    [Header("Verificare Nivel")]
    public HoleFeeding stomachScript; 
    public HoleInventory inventoryScript; 

    [Header("Elemente Vizuale")]
    public LineRenderer lineRenderer;
    public GameObject targetIndicator; 

    [Header("Setări Catapultă")]
    public Transform shootPoint; 
    public float launchAngle = 45f;
    [Tooltip("Forța maximă. Trebuie să fie mai mare (ex: 30-40) pentru a atinge ținte înalte!")]
    public float maxForce = 30f; 

    [Header("Setări Traiectorie (Parabolă)")]
    public int resolution = 30; 
    public float timeStep = 0.05f; 

    [Header("Efecte Vizuale")]
    [Tooltip("Trage aici Prefab-ul cu particulele de explozie")]
    public GameObject explosionEffectPrefab;

    private bool isAiming = false;
    private Vector3 currentVelocity;

    void Start()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (targetIndicator != null) targetIndicator.SetActive(false);
    }

    void Update()
    {
        if (stomachScript == null || !stomachScript.isBossLevel)
        {
            CancelAiming();
            return;
        }

        if (!inventoryScript.HasAmmo())
        {
            CancelAiming();
            return;
        }

        HandleShootingInput();
    }

    void HandleShootingInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isAiming = true;
            if (lineRenderer != null) lineRenderer.enabled = true;
            if (targetIndicator != null) targetIndicator.SetActive(true);
        }

        if (isAiming && Mouse.current.leftButton.isPressed)
        {
            CalculateLaunchVelocity();
            DrawTrajectory();
        }

        if (isAiming && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            FireProjectile();
        }
    }

    void CalculateLaunchVelocity()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 targetPoint = hit.point;
            Vector3 fromPoint = shootPoint.position;

            Vector3 displacement = targetPoint - fromPoint;
            
            // Separăm diferența de înălțime (Y) și distanța pe orizontală (X, Z)
            float h = displacement.y; 
            displacement.y = 0;
            float d = displacement.magnitude; 

            if (d == 0) return;

            float angleRad = launchAngle * Mathf.Deg2Rad;
            float gravity = Mathf.Abs(Physics.gravity.y);

            // Aplicăm formula balistică pentru 3D
            float denominator = 2 * (d * Mathf.Tan(angleRad) - h) * Mathf.Cos(angleRad) * Mathf.Cos(angleRad);
            
            if (denominator > 0)
            {
                // Putem atinge ținta păstrând unghiul curbat
                float force = Mathf.Sqrt((gravity * d * d) / denominator);
                force = Mathf.Clamp(force, 0, maxForce);

                Vector3 velocityDirection = displacement.normalized * Mathf.Cos(angleRad) + Vector3.up * Mathf.Sin(angleRad);
                currentVelocity = velocityDirection * force;
            }
            else
            {
                // Fallback: Ținta e prea sus pentru a face o parabolă. Tragem ca un tun, direct în ea!
                currentVelocity = (targetPoint - fromPoint).normalized * maxForce;
            }
        }
    }

    void DrawTrajectory()
    {
        if (lineRenderer == null) return;

        lineRenderer.positionCount = resolution;
        Vector3 startPosition = shootPoint.position;
        Vector3 lastPoint = startPosition;

        bool hitGround = false;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPosition + currentVelocity * t + 0.5f * Physics.gravity * t * t;

            if (!hitGround && i > 0)
            {
                if (Physics.Linecast(lastPoint, point, out RaycastHit hit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    hitGround = true;
                    point = hit.point;
                    
                    if (targetIndicator != null)
                    {
                        // 1. Dăm ținta un pic în exterior față de perete ca să nu intre în textură
                        targetIndicator.transform.position = hit.point + hit.normal * 0.05f; 
                        
                        // 2. Această linie aliniază perfect cercul pe plan vertical sau orizontal
                        targetIndicator.transform.up = hit.normal; 
                    }
                }
            }

            lineRenderer.SetPosition(i, point);
            lastPoint = point;
        }

        if (!hitGround && targetIndicator != null) targetIndicator.SetActive(false);
        else if (hitGround && targetIndicator != null) targetIndicator.SetActive(true);
    }

    // void FireProjectile()
    // {
    //     GameObject projectile = inventoryScript.ConsumeAmmo();

    //     if (projectile != null)
    //     {
    //         projectile.transform.position = shootPoint.position;
    //         projectile.SetActive(true);
            
    //         if (projectile.GetComponent<ProjectileImpact>() == null)
    //         {
    //             projectile.AddComponent<ProjectileImpact>();
    //         }

    //         Rigidbody rb = projectile.GetComponent<Rigidbody>();
    //         if (rb != null)
    //         {
    //             rb.linearVelocity = Vector3.zero;
    //             rb.angularVelocity = Vector3.zero; 
    //             rb.isKinematic = false; 
    //             rb.useGravity = true;
    //             rb.AddForce(currentVelocity, ForceMode.VelocityChange);
    //         }
    //     }

    //     CancelAiming();
    // }

    void FireProjectile()
    {
        GameObject projectile = inventoryScript.ConsumeAmmo();

        if (projectile != null)
        {
            projectile.transform.position = shootPoint.position;
            projectile.SetActive(true);
            
            // --- COD MODIFICAT: ÎNARMĂM ȘI TRANSMITEM EXPLOZIA ---
            ProjectileImpact impactScript = projectile.GetComponent<ProjectileImpact>();
            if (impactScript == null)
            {
                impactScript = projectile.AddComponent<ProjectileImpact>();
            }

            // Pasăm Prefab-ul de explozie de la catapultă la proiectil
            impactScript.explosionPrefab = explosionEffectPrefab; 
            // ----------------------------------------------------

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero; 
                rb.isKinematic = false; 
                rb.useGravity = true;
                rb.AddForce(currentVelocity, ForceMode.VelocityChange);
            }
        }

        CancelAiming();
    }

    void CancelAiming()
    {
        isAiming = false;
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (targetIndicator != null) targetIndicator.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (shootPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(shootPoint.position, 0.1f);
        }
    }
} // Aici se închide clasa
