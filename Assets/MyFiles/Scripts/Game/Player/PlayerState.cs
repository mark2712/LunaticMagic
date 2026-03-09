using UniRx;
using System;
using UnityEngine;

public class PlayerState
{
    public void Load(PlayerStateData data)
    {

    }

    public PlayerStateData Save()
    {
        PlayerStateData data = new()
        {

        };
        return data;
    }
}

[Serializable]
public class PlayerStateData
{

}

