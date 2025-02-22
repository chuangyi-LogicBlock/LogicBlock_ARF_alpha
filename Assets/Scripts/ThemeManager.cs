using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ThemeData
{
    public string themeName;
    public List<GameObject> personModels;
}

public class ThemeManager : MonoBehaviour
{
    public List<ThemeData> themes;
}