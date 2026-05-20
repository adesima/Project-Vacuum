using UnityEngine;
using System.Collections;

public class HoleAnimation : MonoBehaviour
{
    [Header("Setări Animație")]
    [Tooltip("Cât timp durează ca gaura să se deschidă (în secunde)")]
    public float introDuration = 1f;
    
    [Tooltip("Trasează cum va crește gaura (pune o curbă cu 'bouncing' pentru efect maxim)")]
    public AnimationCurve growthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 targetScale;
    private HoleMovement movementScript;

    void Start()
    {
        // 1. Memorăm cât de mare este gaura în mod normal (cât ai setat-o tu în Inspector)
        targetScale = transform.localScale;

        // 2. Oprim scriptul de mișcare ca jucătorul să nu poată fugi cu gaura cât timp ea se deschide
        movementScript = GetComponent<HoleMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // 3. Facem gaura invizibilă (diametru 0) la secunda 0. 
        // Lăsăm axa Y (adâncimea) la fel, vrem doar să se deschidă ca un cerc pe orizontală.
        transform.localScale = new Vector3(0f, targetScale.y, 0f);

        // 4. Pornim animația
        StartCoroutine(AnimateHoleIntro());
    }

    private IEnumerator AnimateHoleIntro()
    {
        float elapsedTime = 0f;
        Vector3 startScale = new Vector3(0f, targetScale.y, 0f);

        while (elapsedTime < introDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calculăm procentul de finalizare (de la 0.0 la 1.0)
            float percent = elapsedTime / introDuration;
            
            // Folosim curba pentru a adăuga acel efect de "elasticitate"
            float curveValue = growthCurve.Evaluate(percent);

            // Aplicăm noua scară (Vector3.LerpUnclamped permite curbei să depășească 100% pentru efectul de Bounce)
            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);

            // Așteptăm următorul frame
            yield return null; 
        }

        // Ne asigurăm că la final are exact dimensiunea corectă
        transform.localScale = targetScale;

        // Redăm jucătorului controlul găurii
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
    }
}