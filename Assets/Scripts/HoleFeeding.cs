// using UnityEngine;

// public class HoleFeeding : MonoBehaviour
// {
//     [Header("Setări Creștere")]
//     [Tooltip("Cu cât crește gaura după fiecare obiect mâncat")]
//     public float growthAmount = 0.1f; 

//     private void OnTriggerEnter(Collider other)
//     {
//         // 1. Verificăm dacă obiectul are tag-ul "Food"
//         if (other.CompareTag("Food"))
//         {
//             // 2. Îl ștergem din scenă ("Îl mâncăm")
//             Destroy(other.gameObject);

//             // 3. Creștem dimensiunea găurii (Cilindrul Părinte)
//             // Creștem doar lățimea (X și Z), vrem să păstrăm aceeași adâncime (Y)
//             Transform parentHole = transform.parent;
//             parentHole.localScale += new Vector3(growthAmount, 0f, growthAmount);

//             Debug.Log("Yum! Am mâncat un obiect. Noua dimensiune: " + parentHole.localScale.x);
//         }
//     }
// }

// -------------------------------------------------------------------------------------------------------------

// using UnityEngine;
// using System.Collections; // Necesită asta pentru Magnet

// public class HoleConsumption : MonoBehaviour
// {
//     [Header("Referințe")]
//     public HoleMovement holeMovementScript; // Trage cilindrul părinte aici
    
//     [Header("Setări Magnet")]
//     public float magnetForce = 15f;
//     public float magnetDuration = 4f; // Cât durează magnetul
//     public float magnetRadiusMultiplier = 3f; // Cât de mare e raza magnetului față de gaură

//     private Transform parentHole;

//     void Start()
//     {
//         parentHole = transform.parent;
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         // Încercăm să luăm scriptul "SwallowableObject" de pe obiectul care a căzut
//         SwallowableObject food = other.GetComponent<SwallowableObject>();

//         // Dacă obiectul are scriptul, înseamnă că e comestibil
//         if (food != null)
//         {
//             // 1. Creștem dimensiunea în funcție de valoarea setată pe obiect
//             parentHole.localScale += new Vector3(food.growthValue, 0f, food.growthValue);

//             // 2. Verificăm dacă are puteri speciale
//             switch (food.powerUpType)
//             {
//                 case PowerUpType.SpeedBoost:
//                     // Viteză x2 timp de 3 secunde
//                     if (holeMovementScript != null) 
//                         holeMovementScript.ActivateSpeedBoost(2f, 3f); 
//                     break;

//                 case PowerUpType.Magnet:
//                     // Pornim efectul de magnet
//                     StartCoroutine(MagnetRoutine());
//                     break;
                
//                 case PowerUpType.Normal:
//                     // Nu facem nimic special
//                     break;
//             }

//             // 3. Distrugem obiectul mâncat
//             Destroy(other.gameObject);
//         }
//     }

//     // Funcția care atrage obiectele timp de câteva secunde
//     private IEnumerator MagnetRoutine()
//     {
//         float timer = 0f;

//         while (timer < magnetDuration)
//         {
//             // Calculăm o rază de atracție (care crește odată cu gaura)
//             float currentRadius = parentHole.localScale.x * magnetRadiusMultiplier;

//             // Găsim toate obiectele din jurul găurii
//             Collider[] objectsInRadius = Physics.OverlapSphere(parentHole.position, currentRadius);

//             foreach (Collider col in objectsInRadius)
//             {
//                 // Dacă obiectul este comestibil și are fizică...
//                 if (col.GetComponent<SwallowableObject>() != null && col.attachedRigidbody != null)
//                 {
//                     // Îl tragem spre centrul găurii
//                     Vector3 directionToHole = (parentHole.position - col.transform.position).normalized;
//                     col.attachedRigidbody.AddForce(directionToHole * magnetForce * Time.deltaTime, ForceMode.VelocityChange);
//                 }
//             }

//             timer += Time.deltaTime;
//             yield return null; // Așteptăm următorul frame (cadru)
//         }
//     }
// }

using UnityEngine;
using System.Collections; // Necesar pentru corutine

public class HoleFeeding : MonoBehaviour
{
    [Header("Setări Nivel")]
    [Tooltip("Bifează asta doar când jucătorul a ajuns la Boss Fight")]
    public bool isBossLevel = false;
    
    [Header("Referințe")]
    public HoleMovement holeMovementScript;
    public HoleInventory holeInventoryScript;
    
    [Header("Setări Magnet")]
    public float magnetForce = 15f;
    public float magnetDuration = 4f;
    public float magnetRadiusMultiplier = 3f;

    [Header("Setări Fluiditate Creștere")]
    [Tooltip("Cât durează animația de creștere (în secunde)")]
    public float growthDuration = 0.4f;
    [Tooltip("Curba de animație pentru 'feel'-ul creșterii (Ease In Out e bun)")]
    public AnimationCurve growthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Setări Viață (Boss)")]
    [Tooltip("Dimensiunea minimă la care poate ajunge gaura prin damage (implicit 1)")]
    public float minHoleScale = 1f;

    [Header("Efecte Vizuale (Damage)")]
    [Tooltip("Trage aici Prefab-ul cu particulele de fum")]
    public GameObject damageSmokePrefab;

    private Transform parentHole;    
    private Coroutine growthCoroutine;
    private Vector3 intendedTargetScale;

    void Start()
    {
        parentHole = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        SwallowableObject food = other.GetComponent<SwallowableObject>();

        if (food != null)
        {
            // 1. CREȘTEREA ȘI PUTERILE (Valabile mereu, în toate cele 4 zone)
            if (growthCoroutine == null)
            {
                intendedTargetScale = parentHole.localScale;
            }
            intendedTargetScale += new Vector3(food.growthValue, 0f, food.growthValue);

            if (growthCoroutine != null) StopCoroutine(growthCoroutine);
            growthCoroutine = StartCoroutine(SmoothGrowthRoutine(intendedTargetScale));

            switch (food.powerUpType)
            {
                case PowerUpType.SpeedBoost:
                    if (holeMovementScript != null) holeMovementScript.ActivateSpeedBoost(2f, 3f); 
                    break;
                case PowerUpType.Magnet:
                    StartCoroutine(MagnetRoutine());
                    break;
                case PowerUpType.Normal:
                    break;
            }

            // // 2. STOCAREA MUNIȚIEI (Valabilă DOAR în zona de Boss Fight)
            // if (isBossLevel && holeInventoryScript != null)
            // {
            //     holeInventoryScript.AddAmmo();
            // }

            // // 3. Ștergem obiectul de pe hartă pentru a simula "înghițirea"
            // Destroy(other.gameObject);

            // 2 & 3. STOCAREA MUNIȚIEI VS DISTRUGERE
            if (isBossLevel && holeInventoryScript != null)
            {
                // Îi dăm inventarului obiectul real
                holeInventoryScript.AddAmmo(other.gameObject);
                
                // ÎL ASCUNDEM (îl facem invizibil și îi oprim fizica temporar)
                other.gameObject.SetActive(false); 
            }
            else
            {
                // Dacă suntem la un nivel normal de colectare, îl distrugem definitiv ca să nu facă lag
                Destroy(other.gameObject);
            }
        }
    }

    private IEnumerator SmoothGrowthRoutine(Vector3 target)
    {
        float elapsed = 0f;
        Vector3 startScale = parentHole.localScale;

        while (elapsed < growthDuration)
        {
            elapsed += Time.deltaTime;
            // Calculăm procentul (0.0 - 1.0)
            float percent = elapsed / growthDuration;
            
            // Evaluăm curba pentru a decide cât de mult ne-am mișcat
            float curveValue = growthCurve.Evaluate(percent);

            // Interpolăm fluid între 'start' și 'target' pe baza curbei
            parentHole.localScale = Vector3.Lerp(startScale, target, curveValue);
            
            yield return null; // Așteptăm următorul cadru
        }

        // Ne asigurăm că am ajuns fix la țintă la final
        parentHole.localScale = target;
        growthCoroutine = null; // Curățăm referința
    }

    private IEnumerator MagnetRoutine()
    {
        // ... (Păstrează MagnetRoutine exact așa cum era înainte, nu necesită modificări) ...
        // [Aici se termină codul]
        float timer = 0f;
        while (timer < magnetDuration)
        {
            float currentRadius = parentHole.localScale.x * magnetRadiusMultiplier;
            Collider[] objectsInRadius = Physics.OverlapSphere(parentHole.position, currentRadius);
            foreach (Collider col in objectsInRadius)
            {
                if (col.GetComponent<SwallowableObject>() != null && col.attachedRigidbody != null)
                {
                    Vector3 directionToHole = (parentHole.position - col.transform.position).normalized;
                    col.attachedRigidbody.AddForce(directionToHole * magnetForce * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    // --- NOU: Logica de Damage de la Laser ---
    public void TakeShrinkDamage(float damageAmount)
    {
        // 1. Calculăm noua dimensiune (scădem)
        intendedTargetScale -= new Vector3(damageAmount, 0f, damageAmount);

        // Ne asigurăm că nu scade sub minim
        if (intendedTargetScale.x < minHoleScale)
        {
            intendedTargetScale = new Vector3(minHoleScale, parentHole.localScale.y, minHoleScale);
        }

        Debug.Log("💔 Gaura a fost lovită! Dimensiune nouă: " + intendedTargetScale.x);

        // --- NOU: Efectul de Fum la impact ---
        if (damageSmokePrefab != null)
        {
            // Creăm fumul exact pe centrul găurii
            // Îl ridicăm puțin pe axa Y ca să iasă frumos la suprafața ierbii (ex: +0.2f)
            Vector3 smokePosition = parentHole.position + new Vector3(0, 0.2f, 0);
            GameObject smokeEffect = Instantiate(damageSmokePrefab, smokePosition, Quaternion.identity);
            
            // Distrugem obiectul de fum după 2 secunde pentru a nu face lag
            Destroy(smokeEffect, 2f);
        }

        // 3. Pornim animația fluidă de micșorare
        if (growthCoroutine != null) StopCoroutine(growthCoroutine);
        growthCoroutine = StartCoroutine(SmoothGrowthRoutine(intendedTargetScale));
    }
}