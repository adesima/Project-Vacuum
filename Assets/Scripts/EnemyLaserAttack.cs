// using UnityEngine;
// using System.Collections;

// public class EnemyLaserAttack : MonoBehaviour
// {
//     [Header("Referințe Necesare")]
//     [Tooltip("Punctul de unde pleacă laserul (copil al Boss-ului)")]
//     public Transform shootPoint; 
//     [Tooltip("Obiectul cu Line Renderer (LaserBeam)")]
//     public LineRenderer beamLine; 
//     [Tooltip("Obiectul plat de pe pământ (LaserTargetIndicator)")]
//     public Transform targetIndicator; 
    
//     // Referințe către jucător
//     private Transform playerHoleTransform;
//     private HoleFeeding playerFeedingScript;

//     [Header("Setări Atac")]
//     [Tooltip("Cât de repede fuge punctul roșu după gaura ta")]
//     public float trackingSpeed = 3f;
//     [Tooltip("Timpul de încărcare (până se face cercul mare) în secunde")]
//     public float telegraphDuration = 3f;
//     [Tooltip("Cât timp rămâne raza groasă aprinsă")]
//     public float fireDuration = 0.5f;
//     [Tooltip("Timpul dintre atacuri")]
//     public float cooldownDuration = 4f;
//     [Tooltip("Scala maximă a cercului în momentul tragerii")]
//     public float maxIndicatorScale = 3f;
//     [Tooltip("Cât de mult se micșorează gaura la impact (diametru)")]
//     public float shrinkDamage = 0.3f;

//     // Variabile de stare interne
//     private Vector3 currentTargetPosition;
//     private Vector3 initialIndicatorScale;
//     private bool isAttacking = false;
//     private float hitRadius; // Raza în care gaura ia damage

//     void Start()
//     {
//         // Găsim gaura principală (Hole) prin scriptul ei de mișcare
//         HoleMovement mv = FindObjectOfType<HoleMovement>();
//         if (mv != null)
//         {
//             playerHoleTransform = mv.transform;
//             playerFeedingScript = mv.GetComponentInChildren<HoleFeeding>();
//         }

//         initialIndicatorScale = targetIndicator.localScale;
        
//         // Ascundem vizualele la start
//         if(beamLine != null) beamLine.gameObject.SetActive(false);
//         if(targetIndicator != null) targetIndicator.gameObject.SetActive(false);

//         // Pornim bucla infinită de atacuri
//         StartCoroutine(AttackLoop());
//     }

//     void Update()
//     {
//         // Sincronizăm mereu baza laserului cu shootPoint în caz că Boss-ul se mișcă puțin
//         if (beamLine.gameObject.activeInHierarchy && shootPoint != null)
//         {
//             beamLine.SetPosition(0, shootPoint.position);
//         }
//     }

//     private IEnumerator AttackLoop()
//     {
//         // Așteptăm un pic la începutul nivelului
//         yield return new WaitForSeconds(2f);

//         while (true)
//         {
//             // Verificăm dacă suntem la boss level
//             if (playerFeedingScript == null || !playerFeedingScript.isBossLevel)
//             {
//                 yield return new WaitForSeconds(1f);
//                 continue;
//             }

//             // --- PASUL 1: COOLDOWN (Boss-ul se odihnește) ---
//             yield return new WaitForSeconds(cooldownDuration);

//             // --- PASUL 2: URMARE ȘI TELEGRAPH (Încărcare) ---
//             yield return StartCoroutine(TelegraphAndTrackRoutine());

//             // --- PASUL 3: TRAGERE (Fire!) ---
//             yield return StartCoroutine(FireRoutine());
//         }
//     }

//     private IEnumerator TelegraphAndTrackRoutine()
//     {
//         // 1. Activăm elementele vizuale
//         beamLine.gameObject.SetActive(true);
//         targetIndicator.gameObject.SetActive(true);
        
//         // Setează grosimea laserului subțire pentru țintire
//         beamLine.startWidth = 0.05f;
//         beamLine.endWidth = 0.05f;

//         // Resetăm scala indicatorului
//         targetIndicator.localScale = initialIndicatorScale;

//         // --- LOC PENTRU SUNET: Pornire Încărcare ---
//         PlayChargingSound();
//         // -------------------------------------------

//         float elapsed = 0f;
//         // Punctul de start al țintei este poziția actuală a jucătorului
//         currentTargetPosition = new Vector3(playerHoleTransform.position.x, 0.01f, playerHoleTransform.position.z);

//         while (elapsed < telegraphDuration)
//         {
//             elapsed += Time.deltaTime;
//             float percent = elapsed / telegraphDuration;

//             // A. Mișcăm punctul țintă spre jucător cu viteză constantă
//             Vector3 targetPlayerPos = new Vector3(playerHoleTransform.position.x, 0.01f, playerHoleTransform.position.z);
//             currentTargetPosition = Vector3.MoveTowards(currentTargetPosition, targetPlayerPos, trackingSpeed * Time.deltaTime);

//             // B. Actualizăm vizualele
//             targetIndicator.position = currentTargetPosition;
//             beamLine.SetPosition(0, shootPoint.position); // Origine
//             beamLine.SetPosition(1, currentTargetPosition); // Final pe pământ

//             // C. Feedback Vizual: Mărim cercul de pe pământ pe măsură ce se încarcă
//             targetIndicator.localScale = Vector3.Lerp(initialIndicatorScale, initialIndicatorScale * maxIndicatorScale, percent);

//             yield return null; // Așteptăm următorul cadru
//         }
//     }

//     private IEnumerator FireRoutine()
//     {
//         // 1. Facem raza laser foarte groasă
//         beamLine.startWidth = 0.5f;
//         beamLine.endWidth = 0.5f;

//         // --- LOC PENTRU SUNET: Tragere Efectivă ---
//         PlayFiringSound();
//         // -------------------------------------------

//         // 2. Calculăm dacă gaura este în zona de impact
//         // Raza de hit este jumătate din scala indicatorului de pe Y sau X
//         hitRadius = targetIndicator.localScale.x / 2f; 
//         float distanceToHole = Vector3.Distance(new Vector3(playerHoleTransform.position.x, 0, playerHoleTransform.position.z), 
//                                                 new Vector3(currentTargetPosition.x, 0, currentTargetPosition.z));

//         // 3. Verificăm coliziunea
//         if (distanceToHole < hitRadius)
//         {
//             // Dacă gaura este în cerc, aplicăm damage
//             if (playerFeedingScript != null)
//             {
//                 playerFeedingScript.TakeShrinkDamage(shrinkDamage);
//             }
//         }

//         // Așteptăm cât timp raza rămâne aprinsă
//         yield return new WaitForSeconds(fireDuration);

//         // 4. Oprim vizualele
//         beamLine.gameObject.SetActive(false);
//         targetIndicator.gameObject.SetActive(false);
//     }

//     // --- FUNCȚII PENTRU SUNET (De completat ulterior) ---
//     private void PlayChargingSound()
//     {
//         // Aici vei pune codul pentru sunetul de 'zumzet' care crește în intensitate
//         // Ex: audioSource.PlayOneShot(chargingClip);
//         Debug.Log("🔊 Sunet: Laserul se încarcă...");
//     }

//     private void PlayFiringSound()
//     {
//         // Aici vei pune codul pentru sunetul de 'explozie' sau 'zap' puternic
//         // Ex: audioSource.PlayOneShot(firingClip);
//         Debug.Log("🔊 Sunet: ZAP! Laserul a tras!");
//     }
// }

using UnityEngine;
using System.Collections;

public class EnemyLaserAttack : MonoBehaviour
{
    [Header("Referințe Necesare")]
    public Transform shootPoint; 
    public LineRenderer beamLine; 
    public Transform targetIndicator; 
    
    private Transform playerHoleTransform;
    private HoleFeeding playerFeedingScript;

    [Header("Setări Atac")]
    public float trackingSpeed = 3f;
    public float telegraphDuration = 3f;
    [Tooltip("Cât timp rămâne raza de laser pe ecran (un flash scurt, ex: 0.2)")]
    public float fireDuration = 0.2f; 
    public float cooldownDuration = 4f;
    public float maxIndicatorScale = 3f;
    public float shrinkDamage = 0.3f;

    private Vector3 currentTargetPosition;
    private Vector3 initialIndicatorScale;
    private float hitRadius;

    void Start()
    {
        HoleMovement mv = FindObjectOfType<HoleMovement>();
        if (mv != null)
        {
            playerHoleTransform = mv.transform;
            playerFeedingScript = mv.GetComponentInChildren<HoleFeeding>();
        }

        initialIndicatorScale = targetIndicator.localScale;
        
        if(beamLine != null) beamLine.gameObject.SetActive(false);
        if(targetIndicator != null) targetIndicator.gameObject.SetActive(false);

        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (playerFeedingScript == null || !playerFeedingScript.isBossLevel)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            yield return new WaitForSeconds(cooldownDuration);
            yield return StartCoroutine(TelegraphAndTrackRoutine());
            yield return StartCoroutine(FireRoutine());
        }
    }

    private IEnumerator TelegraphAndTrackRoutine()
    {
        // 1. ASCUNDEM LASERUL (se vede doar ținta de pe pământ)
        beamLine.gameObject.SetActive(false);
        targetIndicator.gameObject.SetActive(true);
        targetIndicator.localScale = initialIndicatorScale;

        PlayChargingSound();

        float elapsed = 0f;
        currentTargetPosition = new Vector3(playerHoleTransform.position.x, 0.01f, playerHoleTransform.position.z);

        while (elapsed < telegraphDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / telegraphDuration;

            // Mișcăm punctul țintă
            Vector3 targetPlayerPos = new Vector3(playerHoleTransform.position.x, 0.01f, playerHoleTransform.position.z);
            currentTargetPosition = Vector3.MoveTowards(currentTargetPosition, targetPlayerPos, trackingSpeed * Time.deltaTime);
            
            targetIndicator.position = currentTargetPosition;

            // Mărim cercul
            targetIndicator.localScale = Vector3.Lerp(initialIndicatorScale, initialIndicatorScale * maxIndicatorScale, percent);

            yield return null; 
        }
    }

    private IEnumerator FireRoutine()
    {
        // 1. APRINDEM LASERUL PENTRU FLASH!
        beamLine.gameObject.SetActive(true);
        beamLine.startWidth = 0.8f; // Îl facem gros și periculos
        beamLine.endWidth = 0.8f;
        
        // Desenăm raza exact de la tun până la punctul roșu fixat pe pământ
        beamLine.SetPosition(0, shootPoint.position);
        beamLine.SetPosition(1, currentTargetPosition);

        PlayFiringSound();

        // 2. Calculăm damage-ul
        hitRadius = targetIndicator.localScale.x / 2f; 
        float distanceToHole = Vector3.Distance(new Vector3(playerHoleTransform.position.x, 0, playerHoleTransform.position.z), 
                                                new Vector3(currentTargetPosition.x, 0, currentTargetPosition.z));

        if (distanceToHole < hitRadius)
        {
            if (playerFeedingScript != null)
            {
                playerFeedingScript.TakeShrinkDamage(shrinkDamage);
            }
        }

        // 3. Lăsăm raza pe ecran pentru o fracțiune de secundă (fireDuration)
        yield return new WaitForSeconds(fireDuration);

        // 4. Oprim totul
        beamLine.gameObject.SetActive(false);
        targetIndicator.gameObject.SetActive(false);
    }

    private void PlayChargingSound()
    {
        Debug.Log("🔊 Sunet: Ținta te urmărește...");
    }

    private void PlayFiringSound()
    {
        Debug.Log("🔊 Sunet: PEW! Laser instantaneu!");
    }
}