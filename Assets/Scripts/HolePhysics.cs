using UnityEngine;

public class HolePhysics : MonoBehaviour
{
    [Tooltip("Trage aici cubul tău (Ground) din ierarhie")]
    public Collider groundCollider;

    private void OnTriggerEnter(Collider other)
    {
        // Verificăm dacă obiectul care a atins gaura are un Rigidbody (adică e afectat de gravitație)
        if (other.attachedRigidbody != null)
        {
            // Spunem motorului de fizică să IGNORE coliziunea dintre acest obiect și podea
            Physics.IgnoreCollision(other, groundCollider, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Dacă obiectul a căzut de tot și iese din zona găurii, 
        // resetăm coliziunea la normal pentru a nu lăsa erori în memorie.
        if (other.attachedRigidbody != null)
        {
            Physics.IgnoreCollision(other, groundCollider, false);
        }
    }
}