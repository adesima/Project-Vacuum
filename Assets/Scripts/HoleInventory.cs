// using UnityEngine;
// using System.Collections.Generic;
// using TMPro; 

// public class HoleInventory : MonoBehaviour
// {
//     [Header("Referințe UI")]
//     public TextMeshProUGUI ammoText;

//     [Header("Setări Muniție")]
//     public GameObject projectilePrefab;

//     private List<GameObject> ammoStorage = new List<GameObject>();

//     void Start()
//     {
//         // La începutul jocului, ascundem complet textul de muniție
//         // ToggleUI(false);
//     }

//     public void AddAmmo()
//     {
//         ammoStorage.Add(projectilePrefab);
//         UpdateAmmoUI();
//     }

//     public bool HasAmmo()
//     {
//         return ammoStorage.Count > 0;
//     }

//     public GameObject ConsumeAmmo()
//     {
//         if (ammoStorage.Count > 0)
//         {
//             GameObject proj = ammoStorage[0];
//             ammoStorage.RemoveAt(0);
//             UpdateAmmoUI();
//             return proj;
//         }
//         return null;
//     }

//     private void UpdateAmmoUI()
//     {
//         if (ammoText != null)
//         {
//             ammoText.text = "Munitie: " + ammoStorage.Count;
//         }
//     }

//     // Funcție nouă: Poate aprinde sau stinge textul de pe ecran
//     public void ToggleUI(bool isVisible)
//     {
//         if (ammoText != null)
//         {
//             ammoText.gameObject.SetActive(isVisible);
//             // Dacă îl facem vizibil, actualizăm și numărul ca să fie corect
//             if (isVisible) UpdateAmmoUI(); 
//         }
//     }
// }

using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class HoleInventory : MonoBehaviour
{
    [Header("Referințe UI")]
    public TextMeshProUGUI ammoText;

    // AM ȘTERS VARIABILA 'projectilePrefab' AICI

    private List<GameObject> ammoStorage = new List<GameObject>();

    void Start()
    {
        ToggleUI(false);
    }

    // Acum funcția cere obiectul pe care tocmai l-ai mâncat
    public void AddAmmo(GameObject eatenObject) 
    {
        ammoStorage.Add(eatenObject);
        UpdateAmmoUI();
    }

    public bool HasAmmo()
    {
        return ammoStorage.Count > 0;
    }

    public GameObject ConsumeAmmo()
    {
        if (ammoStorage.Count > 0)
        {
            GameObject proj = ammoStorage[0];
            ammoStorage.RemoveAt(0);
            UpdateAmmoUI();
            return proj;
        }
        return null;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null) ammoText.text = "Munitie: " + ammoStorage.Count;
    }

    public void ToggleUI(bool isVisible)
    {
        if (ammoText != null)
        {
            ammoText.gameObject.SetActive(isVisible);
            if (isVisible) UpdateAmmoUI();
        }
    }
}