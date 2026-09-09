using UnityEngine;
using System.Collections;

public class BreathingSimulationManager : MonoBehaviour
{
    [Header("JSON-Daten")]
    [Tooltip("Hier die Datei breathing_simulation.json aus dem Unity-Projekt hineinziehen.")]
    public TextAsset jsonDatei;

    [Header("Verknuepfungen")]
    public AtemTaktSteuerung atemKugel;
    public MeditationsManager meditationsManager;

    [Header("Auswertung")]
    [Tooltip("Maximal erlaubte Abweichung vom Sollwert fuer jeden Benutzer.")]
    [Range(0.01f, 0.5f)]
    public float toleranz = 0.12f;

    [Tooltip("Wie viel Prozent der Messpunkte eines Atemzyklus bei BEIDEN Personen passen muessen.")]
    [Range(0.5f, 1f)]
    public float benoetigteTrefferquote = 0.8f;

    [Header("Atemzyklus")]
    public int anzahlAtemzyklen = 10;
    public float einatmenDauer = 5f;
    public float ausatmenDauer = 8f;

    private BreathingContainer daten;
    private Coroutine simulationCoroutine;
    private bool simulationLaeuft = false;

    public void StarteSimulation()
    {
        Debug.Log("BreathingSimulationManager: StarteSimulation() wurde aufgerufen.");

        if (simulationLaeuft)
        {
            Debug.LogWarning("BreathingSimulationManager: Simulation laeuft bereits.");
            return;
        }

        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }

        if (!LadeDaten())
        {
            Debug.LogError("BreathingSimulationManager: Simulation konnte nicht gestartet werden.");
            return;
        }

        if (atemKugel == null)
        {
            Debug.LogError("BreathingSimulationManager: Keine Atemkugel zugewiesen.");
            return;
        }

        if (meditationsManager == null)
        {
            Debug.LogError("BreathingSimulationManager: Kein MeditationsManager zugewiesen.");
            return;
        }

        Debug.Log(
            "BreathingSimulationManager: JSON erfolgreich geladen. Messpunkte: "
            + daten.data.Count
        );

        simulationCoroutine = StartCoroutine(SimulationsAblauf());
    }

    public void StoppeSimulation()
    {
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }

        simulationLaeuft = false;

        if (atemKugel != null)
        {
            atemKugel.SetzeAtemWert(0f);
        }

        Debug.Log("BreathingSimulationManager: Simulation wurde gestoppt.");
    }

    private bool LadeDaten()
    {
        if (jsonDatei == null)
        {
            Debug.LogError(
                "BreathingSimulationManager: Keine JSON-Datei zugewiesen."
            );
            return false;
        }

        Debug.Log(
            "BreathingSimulationManager: Versuche JSON zu laden: "
            + jsonDatei.name
        );

        try
        {
            string jsonText = jsonDatei.text;

            // Leerzeichen und eventuell vorhandenes BOM-Zeichen entfernen
            jsonText = jsonText.Trim().TrimStart('\uFEFF');

            if (string.IsNullOrEmpty(jsonText))
            {
                Debug.LogError(
                    "BreathingSimulationManager: Die JSON-Datei ist leer."
                );
                return false;
            }

            // Falls die JSON direkt mit [ beginnt,
            // machen wir daraus automatisch {"data":[...]}
            if (jsonText.StartsWith("["))
            {
                Debug.Log(
                    "BreathingSimulationManager: JSON ist ein Array. " +
                    "Wird automatisch in einen Container verpackt."
                );

                jsonText = "{\"data\":" + jsonText + "}";
            }

            daten = JsonUtility.FromJson<BreathingContainer>(jsonText);

            if (daten == null)
            {
                Debug.LogError(
                    "BreathingSimulationManager: JSON konnte nicht gelesen werden."
                );
                return false;
            }

            if (daten.data == null)
            {
                Debug.LogError(
                    "BreathingSimulationManager: In der JSON wurde kein 'data'-Array gefunden."
                );
                return false;
            }

            if (daten.data.Count == 0)
            {
                Debug.LogError(
                    "BreathingSimulationManager: Die JSON enthält keine Messpunkte."
                );
                return false;
            }

            Debug.Log(
                "JSON ERFOLGREICH GELADEN! Messpunkte: "
                + daten.data.Count
            );

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Fehler beim Lesen der JSON: " + e.Message
            );

            return false;
        }
    }

    private IEnumerator SimulationsAblauf()
    {
        simulationLaeuft = true;

        Debug.Log("BreathingSimulationManager: SimulationsAblauf gestartet.");

        atemKugel.externeSteuerung = true;
        atemKugel.Zuruecksetzen();

        float zyklusDauer = einatmenDauer + ausatmenDauer;

        int aktuellerZyklus = 0;
        int trefferImZyklus = 0;
        int messpunkteImZyklus = 0;

        float simulationsStart = Time.time;

        for (int i = 0; i < daten.data.Count; i++)
        {
            BreathingPoint punkt = daten.data[i];

            float sollZeit = simulationsStart + punkt.time;

            while (Time.time < sollZeit)
            {
                yield return null;
            }

            int punktZyklus =
                Mathf.FloorToInt(punkt.time / zyklusDauer);

            punktZyklus =
                Mathf.Clamp(
                    punktZyklus,
                    0,
                    anzahlAtemzyklen - 1
                );

            while (
                punktZyklus > aktuellerZyklus
                &&
                aktuellerZyklus < anzahlAtemzyklen
            )
            {
                WerteZyklusAus(
                    aktuellerZyklus,
                    trefferImZyklus,
                    messpunkteImZyklus
                );

                aktuellerZyklus++;

                trefferImZyklus = 0;
                messpunkteImZyklus = 0;
            }

            if (aktuellerZyklus >= anzahlAtemzyklen)
            {
                break;
            }

            // Lichtkugel anhand der JSON steuern
            atemKugel.SetzeAtemWert(
                punkt.target_value
            );

            bool person1Passt =
                Mathf.Abs(
                    punkt.p1_value
                    -
                    punkt.target_value
                ) <= toleranz;

            bool person2Passt =
                Mathf.Abs(
                    punkt.p2_value
                    -
                    punkt.target_value
                ) <= toleranz;

            messpunkteImZyklus++;

            if (person1Passt && person2Passt)
            {
                trefferImZyklus++;
            }

            // Optionales Debugging pro Messpunkt
            Debug.Log(
                "Zeit: " + punkt.time.ToString("F1")
                + " | Soll: " + punkt.target_value.ToString("F2")
                + " | P1: " + punkt.p1_value.ToString("F2")
                + " | P2: " + punkt.p2_value.ToString("F2")
            );
        }

        if (aktuellerZyklus < anzahlAtemzyklen)
        {
            WerteZyklusAus(
                aktuellerZyklus,
                trefferImZyklus,
                messpunkteImZyklus
            );

            aktuellerZyklus++;
        }

        while (aktuellerZyklus < anzahlAtemzyklen)
        {
            WerteZyklusAus(
                aktuellerZyklus,
                0,
                0
            );

            aktuellerZyklus++;
        }

        atemKugel.SetzeAtemWert(0f);

        simulationCoroutine = null;
        simulationLaeuft = false;

        Debug.Log(
            "BreathingSimulationManager: Simulation beendet."
        );

        meditationsManager.SimulationBeendet();
    }

    private void WerteZyklusAus(
        int zyklusIndex,
        int treffer,
        int gesamt
    )
    {
        float quote =
            gesamt > 0
            ? (float)treffer / gesamt
            : 0f;

        bool erfolgreich =
            quote >= benoetigteTrefferquote;

        Debug.Log(
            "Atemzyklus "
            + (zyklusIndex + 1)
            + ": "
            + Mathf.RoundToInt(quote * 100f)
            + "% synchron -> "
            + (
                erfolgreich
                ? "ERFOLGREICH"
                : "nicht erfolgreich"
            )
        );

        meditationsManager.AtemzyklusBeendet(
            zyklusIndex + 1,
            erfolgreich
        );
    }
}