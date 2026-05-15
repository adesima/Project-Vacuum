// using UnityEngine;

// public class HoleMovement : MonoBehaviour
// {
//     [Header("Setări Mișcare")]
//     [Tooltip("Cât de repede se mișcă gaura")]
//     public float moveSpeed = 5f;
    
//     [Header("Limite (Limitele Hărții)")]
//     [Tooltip("Distanța maximă pe axa X la care poate ajunge gaura")]
//     public float limitX = 9f;
//     [Tooltip("Distanța maximă pe axa Z la care poate ajunge gaura")]
//     public float limitZ = 9f;

//     void Update()
//     {
//         // 1. Preluăm input-ul. 
//         // "Horizontal" și "Vertical" sunt setate automat de Unity pentru WASD, Săgeți și Joystick (Controller).
//         float moveX = Input.GetAxis("Horizontal");
//         float moveZ = Input.GetAxis("Vertical");

//         // 2. Calculăm direcția și distanța pe care vrem să o parcurgem în acest frame
//         // Folosim Time.deltaTime pentru ca viteza să fie constantă indiferent de FPS
//         Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed * Time.deltaTime;

//         // 3. Calculăm noua poziție dorită
//         Vector3 newPosition = transform.position + movement;

//         // 4. Aplicăm limitele matematice (Clamp)
//         // Asta împiedică gaura să iasă de pe marginea pământului tău
//         newPosition.x = Mathf.Clamp(newPosition.x, -limitX, limitX);
//         newPosition.z = Mathf.Clamp(newPosition.z, -limitZ, limitZ);

//         // Păstrăm poziția Y constantă (gaura nu zboară și nu se scufundă)
//         newPosition.y = transform.position.y;

//         // 5. Aplicăm mișcarea finală pe cilindru
//         transform.position = newPosition;
//     }
// }

using UnityEngine;
using UnityEngine.InputSystem; // <-- Foarte important: aducem noul sistem

public class HoleMovement : MonoBehaviour
{
    [Header("Setări Mișcare")]
    public float moveSpeed = 5f;
    
    [Header("Limite (Limitele Hărții)")]
    public float limitX = 9f;
    public float limitZ = 9f;

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // 1. Verificăm Input-ul de pe Tastatură
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        }

        // 2. Verificăm Input-ul de pe Controller (Gamepad)
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            
            // Adăugăm input-ul de la joystick (dacă tastatura e deja folosită, le combinăm, deși de obicei folosești doar una)
            if (stick.sqrMagnitude > 0.01f) // un mic deadzone
            {
                moveX = stick.x;
                moveZ = stick.y;
            }
        }

        // 3. Calculăm mișcarea (la fel ca înainte)
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + movement;

        newPosition.x = Mathf.Clamp(newPosition.x, -limitX, limitX);
        newPosition.z = Mathf.Clamp(newPosition.z, -limitZ, limitZ);
        newPosition.y = transform.position.y;

        transform.position = newPosition;
    }
}