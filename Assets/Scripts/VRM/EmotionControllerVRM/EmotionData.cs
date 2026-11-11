using System.Collections.Generic;

[System.Serializable]
public class Emotion
{
    public Dictionary<string, float> blendShapeValues;

    public Emotion(Dictionary<string, float> blendShapeValues)
    {
        this.blendShapeValues = blendShapeValues ?? new Dictionary<string, float>();
    }
}

public static class EmotionDatabase
{
    public static readonly Dictionary<string, Emotion> Emotions = new Dictionary<string, Emotion>
    {
        { "Neutral", new Emotion(new Dictionary<string, float>
            {
                { "Fcl_ALL_Neutral", 100f }
            })
        },
        { "Angry", new Emotion(new Dictionary<string, float>
            {
                { "Fcl_ALL_Angry", 100f },
                { "Fcl_BRW_Angry", 50f },
                { "Fcl_EYE_Angry", 30f }
            })
        },
        { "Happy", new Emotion(new Dictionary<string, float>
            {
                { "Fcl_ALL_Happy", 100f },
                { "Fcl_MTH_Smile", 80f },
                { "Fcl_EYE_Happy", 40f }
            })
        },
        { "Sad", new Emotion(new Dictionary<string, float>
            {
                { "Fcl_ALL_Sad", 100f },
                { "Fcl_BRW_Sad", 60f },
                { "Fcl_MTH_Sad", 30f }
            })
        },
        { "Surprised", new Emotion(new Dictionary<string, float>
            {
                { "Fcl_ALL_Surprised", 100f },
                { "Fcl_MTH_O", 50f },
            })
        }
    };
}



/*
Fcl_ALL_Neutral
Fcl_ALL_Angry
Fcl_ALL_Fun
Fcl_ALL_Joy
Fcl_ALL_Sorrow
Fcl_ALL_Surprised
Fcl_BRW_Angry
Fcl_BRW_Fun
Fcl_BRW_Joy
Fcl_BRW_Sorrow
Fcl_BRW_Surprised
Fcl_EYE_Angry
Fcl_EYE_Close
Fcl_EYE_Close_R
Fcl_EYE_Close_L
Fcl_EYE_Fun
Fcl_EYE_Joy
Fcl_EYE_Joy_R
Fcl_EYE_Joy_L
Fcl_EYE_Sorrow
Fcl_EYE_Surprised
Fcl_EYE_Spread
Fcl_EYE_Iris_Hide
Fcl_EYE_Highlight_Hide
Fcl_MTH_Close
Fcl_MTH_Up
Fcl_MTH_Down
Fcl_MTH_Angry
Fcl_MTH_Small
Fcl_MTH_Large
Fcl_MTH_Neutral
Fcl_MTH_Fun
Fcl_MTH_Joy
Fcl_MTH_Sorrow
Fcl_MTH_Surprised
Fcl_MTH_SkinFung
Fcl_MTH_SkinFung_R
Fcl_MTH_SkinFung_L
Fcl_MTH_A
Fcl_MTH_I
Fcl_MTH_U
Fcl_MTH_E
Fcl_MTH_O
Fcl_HA_Hide
Fcl_HA_Fung1
Fcl_HA_Fung1_Low
Fcl_HA_Fung1_Up
Fcl_HA_Fung2
Fcl_HA_Fung2_Low
Fcl_HA_Fung2_Up
Fcl_HA_Fung3
Fcl_HA_Fung3_Up
Fcl_HA_Fung3_Low
Fcl_HA_Short
Fcl_HA_Short_Up
Fcl_HA_Short_Low
*/