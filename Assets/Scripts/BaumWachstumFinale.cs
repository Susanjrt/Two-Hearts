using UnityEngine;
using System.Collections.Generic;

public class BaumWachstumFinale : MonoBehaviour
{
    [Header("Baumstufen")]
    public List<GameObject> baumStufenPrefabs =
        new List<GameObject>();

    [Header("Größe")]
    public float startGroesse = 0.5f;
    public float endGroesse = 3.5f;

    private GameObject aktuellerBaum;
    private GameObject erde;

    private int aktuelleStufe = 0;

    void Start()
    {
        ErstelleErde();
    }

    void ErstelleErde()
    {
        if (erde != null)
            return;

        erde =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        erde.name = "Erde";

        erde.transform.SetParent(transform);

        erde.transform.localPosition =
            Vector3.zero;

        erde.transform.localScale =
            new Vector3(
                1.2f,
                0.2f,
                1.2f
            );

        Renderer renderer =
            erde.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color =
                new Color(
                    0.28f,
                    0.18f,
                    0.10f
                );
        }
    }

    public void ErfolgreicheRunde()
    {
        if (
            baumStufenPrefabs == null ||
            baumStufenPrefabs.Count == 0
        )
        {
            Debug.LogWarning(
                "Keine Baumstufen eingetragen."
            );

            return;
        }

        if (
            aktuelleStufe >=
            baumStufenPrefabs.Count
        )
        {
            return;
        }

        if (aktuellerBaum != null)
        {
            Destroy(aktuellerBaum);
        }

        GameObject prefab =
            baumStufenPrefabs[
                aktuelleStufe
            ];

        aktuellerBaum =
            Instantiate(
                prefab,
                transform.position,
                Quaternion.identity,
                transform
            );

        float fortschritt;

        if (baumStufenPrefabs.Count > 1)
        {
            fortschritt =
                (float)aktuelleStufe /
                (baumStufenPrefabs.Count - 1);
        }
        else
        {
            fortschritt = 1f;
        }

        float groesse =
            Mathf.Lerp(
                startGroesse,
                endGroesse,
                fortschritt
            );

        aktuellerBaum.transform.localScale =
            Vector3.one * groesse;

        aktuelleStufe++;
    }

    public void Zuruecksetzen()
    {
        aktuelleStufe = 0;

        if (aktuellerBaum != null)
        {
            Destroy(aktuellerBaum);
            aktuellerBaum = null;
        }

        ErstelleErde();
    }
}