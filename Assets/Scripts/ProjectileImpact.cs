// using UnityEngine;

// public class ProjectileImpact : MonoBehaviour
// {
//     private bool hasHitSomething = false;

//     // Această funcție se declanșează automat când obiectul se lovește fizic de altceva
//     void OnCollisionEnter(Collision collision)
//     {
//         // Vrem ca efectul să se aplice doar la PRIMA lovitură, nu la fiecare țopăială pe pământ
//         if (hasHitSomething) return;

//         // Cazul 1: A lovit Boss-ul
//         if (collision.gameObject.CompareTag("Enemy"))
//         {
//             // Aici vom apela mai târziu scriptul de viață al Boss-ului
//             Debug.Log("💥 Lovitură directă! Boss-ul a luat damage!");
            
//             // Distrugem proiectilul instantaneu
//             Destroy(gameObject);
//         }
//         // Cazul 2: A lovit pământul, un perete, sau orice altceva
//         else
//         {
//             hasHitSomething = true;

//             // 1. Îi ștergem "cartea de identitate" de mâncare, ca să nu mai poată fi înghițit
//             SwallowableObject foodScript = GetComponent<SwallowableObject>();
//             if (foodScript != null)
//             {
//                 Destroy(foodScript);
//             }

//             // 2. Programăm autodistrugerea peste 2 secunde (timp în care se va rostogoli fizic)
//             Destroy(gameObject, 2f);
//         }
//     }
// }

// ----------------------- varianta buna -------------------------------

// using UnityEngine;

// public class ProjectileImpact : MonoBehaviour
// {
//     private bool hasHitSomething = false;

//     void OnCollisionEnter(Collision collision)
//     {
//         if (hasHitSomething) return;

//         // Cazul 1: A lovit Boss-ul
//         if (collision.gameObject.CompareTag("Enemy"))
//         {
//             Debug.Log("💥 Lovitură directă!");

//             // Căutăm componenta de viață pe obiectul lovit
//             BossHealth boss = collision.gameObject.GetComponent<BossHealth>();
//             if (boss != null)
//             {
//                 // Îi scădem 1 punct de viață (fiecare obiect dă 1 damage)
//                 boss.TakeDamage(1); 
//             }

//             Destroy(gameObject);
//         }
//         // Cazul 2: A lovit iarba / a ratat
//         else
//         {
//             hasHitSomething = true;

//             // 1. Nu mai poate fi stocat în inventar
//             SwallowableObject foodScript = GetComponent<SwallowableObject>();
//             if (foodScript != null) Destroy(foodScript);

//             // 2. Schimbăm fizica obiectului pe layer-ul care "simte" capacul găurii
//             gameObject.layer = LayerMask.NameToLayer("Consumed Ammo");

//             // 3. Obiectul continuă să sară și să se rostogolească, apoi se evaporă după 2 secunde
//             Destroy(gameObject, 2f);
//         }
//     }
// }

using UnityEngine;

public class ProjectileImpact : MonoBehaviour
{
    // --- NOU: Aici vom stoca referința primită de la HoleShooting ---
    [HideInInspector] // Nu vrem să o vedem în Inspector, o gestionăm doar prin cod
    public GameObject explosionPrefab; 
    // -----------------------------------------------------------------

    private bool hasHitSomething = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasHitSomething) return;

        // Cazul 1: A lovit Boss-ul
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasHitSomething = true; // Oprim alte coliziuni

            // Căutăm componenta de viață pe obiectul lovit
            BossHealth boss = collision.gameObject.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(1); 
            }

            // --- COD NOU: DECLANȘĂM EXPLOZIA ---
            if (explosionPrefab != null)
            {
                // Instanțiem (creăm) explozia la locul impactului
                // Folosim Quaternion.identity pentru rotație default (zero)
                GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                
                // Ne asigurăm că explozia se distruge singură după ce termină (ex: 2 secunde)
                // Asta previne lag-ul în joc
                Destroy(effect, 2f); 
            }
            // -----------------------------------
            
            // Distrugem proiectilul (bomba dispare)
            Destroy(gameObject);
        }
        // Cazul 2: A lovit altceva (pământ, perete) - păstrăm logica anterioară
        else
        {
            // ... (restul codului cu rostogolirea peste capac rămâne la fel) ...
            hasHitSomething = true;
            SwallowableObject foodScript = GetComponent<SwallowableObject>();
            if (foodScript != null) Destroy(foodScript);
            gameObject.layer = LayerMask.NameToLayer("MunitieConsumata");
            Destroy(gameObject, 2f);
        }
    }
}