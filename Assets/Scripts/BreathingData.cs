using System;
using System.Collections.Generic;

[System.Serializable]
public class BreathingPoint
{
    public float time;
    public float target_value;
    public float p1_value;
    public float p2_value;
}

[System.Serializable]
public class BreathingContainer
{
    public List<BreathingPoint> data;
}
