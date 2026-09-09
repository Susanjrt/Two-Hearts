# Two-Hearts
SPE Projekt XR 

**Two Hearts** ist ein im Rahmen eines zweiwöchigen SPE-Projekts entwickelter Unity-Prototyp zum Thema **XR und Biofeedback**.

Ziel der Anwendung ist es, zwei Personen dabei zu unterstützen, ihre Atmung aufeinander abzustimmen. Die gemeinsame Atmung wird innerhalb einer meditativen Umgebung visualisiert.

## Projektidee

Zwei Personen folgen gemeinsam einem vorgegebenen Atemrhythmus. Eine Lichtkugel visualisiert den aktuellen Sollwert der Atmung.

Die Atemdaten beider Personen werden miteinander und mit dem vorgegebenen Atemwert verglichen. Stimmen beide Personen ausreichend mit dem Sollwert überein, wird der Atemzyklus als erfolgreich gewertet.

Jeder erfolgreiche Atemzyklus lässt einen Baum weiter wachsen.

Das Ziel ist es, durch gemeinsame und synchronisierte Atmung den Baum vollständig wachsen zu lassen.

## Ablauf

1. Start der Meditation
2. 5-Sekunden-Countdown
3. Beginn der Atemsimulation
4. Visualisierung des Atemrhythmus durch eine Lichtkugel
5. Vergleich der Atemwerte beider Personen mit dem Sollwert
6. Bewertung des Atemzyklus
7. Wachstum des Baumes bei erfolgreichem Atemzyklus
8. Finale nach drei erfolgreichen Atemzyklen

Ein Atemzyklus besteht aus:

- **5 Sekunden Einatmen**
- **8 Sekunden Ausatmen**

Insgesamt können bis zu **10 Atemzyklen** durchgeführt werden.

## Atemsimulation

Im aktuellen Prototyp werden noch keine realen Sensoren verwendet.

Die Atemdaten werden über eine **JSON-Datei simuliert**. Jeder Messpunkt enthält:

| Feld | Bedeutung |
|---|---|
| `time` | Zeitpunkt seit Simulationsstart in Sekunden |
| `target_value` | Sollwert der Atembewegung |
| `p1_value` | simulierter Atemwert von Person 1 |
| `p2_value` | simulierter Atemwert von Person 2 |

Die Atemwerte sind normalisierte, einheitslose Werte im Bereich von **0 bis 1**.

Die Architektur wurde so aufgebaut, dass die simulierte Datenquelle zukünftig durch reale Sensordaten, eine API oder eine Datenbank ersetzt werden kann.

## Wichtige Komponenten

### `MeditationsManager`

Steuert den allgemeinen Ablauf der Meditation, unter anderem:

- Start und Countdown
- Anzeige der Atemzyklen und Erfolge
- Kommunikation mit der Atemsimulation
- Baumwachstum
- Finale
- Neustart der Anwendung

### `BreathingSimulationManager`

Verarbeitet die simulierten Atemdaten und bewertet die Synchronität der beiden Personen.

Besonders relevante Methoden sind:

- `LadeDaten()` – lädt die simulierten Atemdaten
- `SimulationsAblauf()` – verarbeitet die Messwerte
- `WerteZyklusAus()` – bewertet einen abgeschlossenen Atemzyklus

### `AtemTaktSteuerung`

Visualisiert den vorgegebenen Atemwert mithilfe der Lichtkugel.

### `BaumWachstumFinale`

Steuert die einzelnen Wachstumsstufen des Baumes.

## Zukünftige Erweiterung

Für eine reale Anwendung kann die derzeitige JSON-Simulation durch eine externe Datenquelle ersetzt werden.

Möglicher zukünftiger Datenfluss:

Sensoren → Datenbank/API → Unity → Auswertung → Visualisierung

Die bestehende Meditations-, Erfolgs- und Visualisierungslogik kann dabei weitgehend erhalten bleiben.

## Projekt öffnen

Das Projekt wurde mit **Unity 6** entwickelt.

Zum Öffnen:

1. Repository klonen
2. Projekt über Unity Hub hinzufügen
3. Passende Unity-Version auswählen
4. Projekt öffnen
5. `MainScene` öffnen
6. Play Mode starten

Unity erzeugt lokale Ordner wie `Library`, `Temp` und `Logs` beim ersten Öffnen automatisch.

## Hinweis zu externen Assets

Einige externe Assets, insbesondere Musik- und Sounddateien, sind aus Lizenz- und Speichergründen nicht Bestandteil dieses Repositorys.

Diese müssen bei Bedarf separat in das Unity-Projekt importiert werden.

## Projektstatus

**Prototyp / Proof of Concept**

Der aktuelle Stand demonstriert insbesondere die Atemsimulation, Synchronitätsbewertung, visuelle Rückmeldung und das gemeinsame Baumwachstum.
