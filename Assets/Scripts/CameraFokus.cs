using UnityEngine;

public class CameraFokus : MonoBehaviour
{
    // Hier ziehen wir gleich den Baum-Spawner hinein
    public Transform zielObjekt;

    // Abstand der Kamera zum Baum (X = zur Seite, Y = Höhe, Z = nach vorne/hinten)
    public Vector3 abstand = new Vector3(0f, 3f, -5f);

    void LateUpdate()
    {
        if (zielObjekt != null)
        {
            // Setzt die Kamera in den perfekten Abstand zum Spawner
            transform.position = zielObjekt.position + abstand;

            // Zwingt die Kamera, sich mathematisch exakt zum Baum umzudrehen
            transform.LookAt(zielObjekt.position + Vector3.up * 1f);
        }
    }
}
