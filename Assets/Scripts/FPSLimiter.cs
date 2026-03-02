using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [System.Obsolete]
    void Start()
    {
        // Exemple : limite à 30 FPS
        //Application.targetFrameRate = 30;

        // Ou 60 FPS
        //Application.targetFrameRate = 60;

        // Ou utiliser le refresh natif de l'écran (si tu veux le max)
         Application.targetFrameRate = Screen.currentResolution.refreshRate;        
    }
}
