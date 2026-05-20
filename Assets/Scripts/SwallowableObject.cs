using UnityEngine;

// Creăm o listă cu tipurile posibile de obiecte
public enum PowerUpType
{
    Normal,
    SpeedBoost,
    Magnet
}

public class SwallowableObject : MonoBehaviour
{
    [Header("Proprietăți Obiect")]
    [Tooltip("Cât de mult crește gaura când înghite asta (ex: 0.05, 0.1, 0.2)")]
    public float growthValue = 0.05f;
    
    [Tooltip("Ce efect special are acest obiect?")]
    public PowerUpType powerUpType = PowerUpType.Normal;
}