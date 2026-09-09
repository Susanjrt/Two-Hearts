using UnityEngine;

public class VogelFlug : MonoBehaviour
{
    public float geschwindigkeit = 2f;
    public float flugStrecke = 30f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * geschwindigkeit * Time.deltaTime, Space.World);

        if (Vector3.Distance(startPosition, transform.position) >= flugStrecke)
        {
            transform.position = startPosition;
        }
    }
}