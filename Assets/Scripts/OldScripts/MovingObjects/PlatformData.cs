using System;
using UnityEngine;

[Serializable]
public class PlatformData
{
    public string PlatformId;

    public void Load(string data)
    {
        JsonUtility.FromJsonOverwrite(data, this);
    }

    public string Save()
    {
        return JsonUtility.ToJson(this, true);
    }
}