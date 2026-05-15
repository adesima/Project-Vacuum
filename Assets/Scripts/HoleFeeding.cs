using UnityEngine;

public class HoleFeeding : MonoBehaviour
{
    [Header("Setări Creștere")]
    [Tooltip("Cu cât crește gaura după fiecare obiect mâncat")]
    public float growthAmount = 0.1f; 

    private void OnTriggerEnter(Collider other)
    {
        // 1. Verificăm dacă obiectul are tag-ul "Food"
        if (other.CompareTag("Food"))
        {
            // 2. Îl ștergem din scenă ("Îl mâncăm")
            Destroy(other.gameObject);

            // 3. Creștem dimensiunea găurii (Cilindrul Părinte)
            // Creștem doar lățimea (X și Z), vrem să păstrăm aceeași adâncime (Y)
            Transform parentHole = transform.parent;
            parentHole.localScale += new Vector3(growthAmount, 0f, growthAmount);

            Debug.Log("Yum! Am mâncat un obiect. Noua dimensiune: " + parentHole.localScale.x);
        }
    }
}