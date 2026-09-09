using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class MeditationsManager : MonoBehaviour
{
    [Header("UI Elemente")]
    public GameObject startButtonObjekt;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI rundenText;
    public TextMeshProUGUI erfolgsText;

    [Header("Meditations-Objekte")]
    public GameObject baumSpawner;
    public GameObject atemKugel;
    public BreathingSimulationManager simulationManager;

    [Header("Einblend-Effekte")]
    public Light sonnenLicht;
    public float einblendDauer = 3f;
    public float sonnenHelligkeit = 2f;

    [Header("Spielregeln")]
    public int benoetigteErfolge = 3;
    public int gesamtAtemzyklen = 10;


    [Header("Finale")]
    public Image rosaOverlay;
    public float overlayEinblendDauer = 2f;
    public float finaleDauer = 6f;

    private int erfolge = 0;
    private bool finaleGestartet = false;

    public GameObject erklaerungsText;

    void Start()
    {
        if (baumSpawner != null)
            baumSpawner.SetActive(false);

        if (atemKugel != null)
            atemKugel.SetActive(false);

        if (sonnenLicht != null)
            sonnenLicht.intensity = 0f;

        if (countdownText != null)
            countdownText.text = "";

        if (rundenText != null)
            rundenText.text = "";

        if (erfolgsText != null)
            erfolgsText.text = "";

        finaleGestartet = false;

        // Rosa Overlay am Anfang unsichtbar
        if (rosaOverlay != null)
        {
            Color c = rosaOverlay.color;
            c.a = 0f;
            rosaOverlay.color = c;
        }
    }

    public void StarteMeditation()
    {
        erfolge = 0;
        finaleGestartet = false;

        if (erklaerungsText != null)
        {
            erklaerungsText.SetActive(false);
        }

        // Sicherstellen, dass das Overlay beim Neustart weg ist
        if (rosaOverlay != null)
        {
            Color c = rosaOverlay.color;
            c.a = 0f;
            rosaOverlay.color = c;
        }

        if (startButtonObjekt != null)
        {
            Button buttonComp =
                startButtonObjekt.GetComponent<Button>();

            if (buttonComp != null)
                buttonComp.interactable = false;
        }

        StartCoroutine(CountdownAblauf());
    }

    IEnumerator CountdownAblauf()
    {
        int verbleibendeZeit = 5;

        while (verbleibendeZeit > 0)
        {
            if (countdownText != null)
                countdownText.text =
                    verbleibendeZeit.ToString();

            yield return new WaitForSeconds(1f);

            verbleibendeZeit--;
        }

        if (countdownText != null)
            countdownText.text = "";

        if (startButtonObjekt != null)
            startButtonObjekt.SetActive(false);

        if (baumSpawner != null)
            baumSpawner.SetActive(true);

        yield return StartCoroutine(
            SanftesEinblendenRoutine()
        );

        if (atemKugel != null)
            atemKugel.SetActive(true);

        if (simulationManager != null)
            simulationManager.StarteSimulation();
    }

    IEnumerator SanftesEinblendenRoutine()
    {
        // Texte erscheinen gleichzeitig mit dem Sonnenaufgang
        if (rundenText != null)
        {
            rundenText.text =
                "Atemzug 1 / " +
                gesamtAtemzyklen;
        }

        if (erfolgsText != null)
        {
            erfolgsText.text =
                "Gemeinsame Erfolge: 0 / " +
                benoetigteErfolge;
        }

        float verstricheneZeit = 0f;

        while (verstricheneZeit < einblendDauer)
        {
            verstricheneZeit += Time.deltaTime;

            float t =
                verstricheneZeit / einblendDauer;

            if (sonnenLicht != null)
            {
                sonnenLicht.intensity =
                    Mathf.Lerp(
                        0f,
                        sonnenHelligkeit,
                        t
                    );
            }

            yield return null;
        }

        if (sonnenLicht != null)
        {
            sonnenLicht.intensity =
                sonnenHelligkeit;
        }
    }

    public void AtemzyklusBeendet(
        int atemzyklusNummer,
        bool erfolgreich
    )
    {
        // Wenn Finale schon läuft, keine weiteren Runden mehr auswerten
        if (finaleGestartet)
            return;

        if (rundenText != null)
        {
            rundenText.text =
                "Atemzug: " +
                atemzyklusNummer +
                " / " +
                gesamtAtemzyklen;
        }

        if (erfolgreich)
        {
            erfolge++;

            if (erfolgsText != null)
            {
                erfolgsText.text =
                    "Gemeinsame Erfolge: " +
                    erfolge +
                    " / " +
                    benoetigteErfolge;
            }

            // Baum wachsen lassen
            if (baumSpawner != null)
            {
                BaumWachstumFinale baum =
                    baumSpawner
                    .GetComponent<BaumWachstumFinale>();

                if (baum != null)
                {
                    baum.ErfolgreicheRunde();
                }
            }

            // Finale starten, sobald genug Erfolge erreicht wurden
            if (
                erfolge >= benoetigteErfolge &&
                !finaleGestartet
            )
            {
                StartCoroutine(FinaleAblauf());
            }
        }
    }

    IEnumerator FinaleAblauf()
    {
        finaleGestartet = true;

        // Simulation stoppen
        if (simulationManager != null)
        {
            simulationManager.StoppeSimulation();
        }

        // Atemkugel ausblenden
        if (atemKugel != null)
        {
            atemKugel.SetActive(false);
        }

        // Finale-Texte
        if (rundenText != null)
        {
            rundenText.text =
                "Gemeinsam im Rhythmus";
        }

        if (erfolgsText != null)
        {
            erfolgsText.text =
                "Geschafft! " +
                erfolge +
                " / " +
                benoetigteErfolge;
        }

        // Rosa Overlay weich einblenden
        if (rosaOverlay != null)
        {
            float zeit = 0f;

            Color startFarbe =
                rosaOverlay.color;

            startFarbe.a = 0f;

            Color zielFarbe =
                rosaOverlay.color;

            // 0.35 = relativ transparent,
            // damit Baum und Landschaft sichtbar bleiben
            zielFarbe.a = 0.35f;

            while (zeit < overlayEinblendDauer)
            {
                zeit += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        zeit / overlayEinblendDauer
                    );

                rosaOverlay.color =
                    Color.Lerp(
                        startFarbe,
                        zielFarbe,
                        t
                    );

                yield return null;
            }

            rosaOverlay.color =
                zielFarbe;
        }

        // Finale anzeigen
        yield return new WaitForSeconds(
            finaleDauer
        );

        // Automatisch zurück zum Start
        Neustart();
    }

    public void SimulationBeendet()
    {
        // Wenn das Finale schon läuft,
        // soll die Simulation nichts mehr überschreiben
        if (finaleGestartet)
            return;

        if (rundenText != null)
        {
            rundenText.text =
                "Meditation beendet";
        }

        if (erfolge >= benoetigteErfolge)
        {
            if (erfolgsText != null)
            {
                erfolgsText.text =
                    "Geschafft! " +
                    erfolge +
                    " / " +
                    benoetigteErfolge;
            }
        }
        else
        {
            if (erfolgsText != null)
            {
                erfolgsText.text =
                    "Erreicht: " +
                    erfolge +
                    " / " +
                    benoetigteErfolge;
            }
        }
    }

    public void Neustart()
    {
        // Laufende Coroutines abbrechen
        StopAllCoroutines();

        // JSON-Simulation stoppen
        if (simulationManager != null)
        {
            simulationManager.StoppeSimulation();
        }

        erfolge = 0;
        finaleGestartet = false;

        // Baum zurücksetzen
        if (baumSpawner != null)
        {
            BaumWachstumFinale baum =
                baumSpawner
                .GetComponent<BaumWachstumFinale>();

            if (baum != null)
            {
                baum.Zuruecksetzen();
            }

            baumSpawner.SetActive(false);
        }

        // Atemkugel zurücksetzen
        if (atemKugel != null)
        {
            AtemTaktSteuerung kugel =
                atemKugel
                .GetComponent<AtemTaktSteuerung>();

            if (kugel != null)
            {
                kugel.Zuruecksetzen();
            }

            atemKugel.SetActive(false);
        }

        // Sonne wieder ausschalten
        if (sonnenLicht != null)
        {
            sonnenLicht.intensity = 0f;
        }

        // Texte entfernen
        if (countdownText != null)
            countdownText.text = "";

        if (rundenText != null)
            rundenText.text = "";

        if (erfolgsText != null)
            erfolgsText.text = "";

        // Rosa Overlay wieder unsichtbar
        if (rosaOverlay != null)
        {
            Color c = rosaOverlay.color;
            c.a = 0f;
            rosaOverlay.color = c;
        }

        // Startbutton wieder anzeigen
        if (startButtonObjekt != null)
        {
            startButtonObjekt.SetActive(true);

            Button button =
                startButtonObjekt
                .GetComponent<Button>();

            if (button != null)
            {
                button.interactable = true;
            }
        }

        Debug.Log(
            "Meditation wurde zurückgesetzt."
        );
    }
}