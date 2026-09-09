using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void SpielBeenden()
    {
        Debug.Log("Spiel wird beendet.");

#if UNITY_EDITOR
        // Im Unity Editor den Play-Modus beenden
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Im fertigen Spiel die Anwendung schließen
        Application.Quit();
#endif
    }
}