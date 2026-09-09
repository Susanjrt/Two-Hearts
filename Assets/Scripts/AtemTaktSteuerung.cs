using UnityEngine;

public class AtemTaktSteuerung : MonoBehaviour
{
    [Header("Größen-Einstellungen")]
    public float minimaleGroesse = 0.3f;
    public float maximaleGroesse = 0.9f;

    [Header("Sanfte Bewegung")]
    public float glaettungsZeit = 0.35f;

    [Header("Leuchten")]
    public Color leuchtFarbe = new Color(1f, 0.8f, 0.2f);
    public float minimaleLeuchtStaerke = 0.4f;
    public float maximaleLeuchtStaerke = 2.5f;

    [HideInInspector]
    public bool externeSteuerung = false;

    private Light punktLicht;
    private Renderer kugelRenderer;
    private Material kugelMaterial;

    private float gemerkteMaximalIntensitaet = 1000f;

    private float zielAtemWert = 0f;
    private float aktuellerAtemWert = 0f;
    private float geschwindigkeit = 0f;

    void Start()
    {
        punktLicht = GetComponentInChildren<Light>();

        if (punktLicht != null)
        {
            gemerkteMaximalIntensitaet = punktLicht.intensity;
            punktLicht.transform.localPosition = Vector3.zero;
        }

        kugelRenderer = GetComponent<Renderer>();

        if (kugelRenderer != null)
        {
            // Eigene Material-Instanz für diese Kugel
            kugelMaterial = kugelRenderer.material;

            // Emission aktivieren
            kugelMaterial.EnableKeyword("_EMISSION");
        }

        Zuruecksetzen();
    }

    void Update()
    {
        aktuellerAtemWert = Mathf.SmoothDamp(
            aktuellerAtemWert,
            zielAtemWert,
            ref geschwindigkeit,
            glaettungsZeit
        );

        float weicherWert = Mathf.SmoothStep(
            0f,
            1f,
            aktuellerAtemWert
        );

        // Größe
        float aktuelleGroesse = Mathf.Lerp(
            minimaleGroesse,
            maximaleGroesse,
            weicherWert
        );

        transform.localScale = Vector3.one * aktuelleGroesse;

        // Point Light
        if (punktLicht != null)
        {
            punktLicht.intensity =
                weicherWert * gemerkteMaximalIntensitaet;
        }

        // Material-Emission
        if (kugelMaterial != null)
        {
            float emissionStaerke = Mathf.Lerp(
                minimaleLeuchtStaerke,
                maximaleLeuchtStaerke,
                weicherWert
            );

            Color emissionColor =
                leuchtFarbe * emissionStaerke;

            kugelMaterial.SetColor(
                "_EmissionColor",
                emissionColor
            );
        }
    }

    public void SetzeAtemWert(float wert)
    {
        zielAtemWert = Mathf.Clamp01(wert);
    }

    public void Zuruecksetzen()
    {
        zielAtemWert = 0f;
        aktuellerAtemWert = 0f;
        geschwindigkeit = 0f;

        transform.localScale =
            Vector3.one * minimaleGroesse;

        if (punktLicht != null)
        {
            punktLicht.intensity = 0f;
        }

        if (kugelMaterial != null)
        {
            kugelMaterial.SetColor(
                "_EmissionColor",
                leuchtFarbe * minimaleLeuchtStaerke
            );
        }
    }
}