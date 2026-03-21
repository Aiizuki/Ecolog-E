using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolSeetings", menuName = "Scriptable Objects/PoolSettings")]
public class PoolSettings : ScriptableObject
{
    public List<GameObject> lstObject;

    [Min(0)] public int initialSize = 10;
    [Min(1)] public int maxSize = 50;
}
