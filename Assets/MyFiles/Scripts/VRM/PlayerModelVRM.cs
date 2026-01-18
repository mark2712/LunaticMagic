using UnityEngine;
using UniVRM10;

public class PlayerModelVRM
{
    public Transform playerModel;
    public Transform model;
    public Vrm10Instance modelVRM;

    public PlayerModelVRM(Transform playerModel)
    {
        this.playerModel = playerModel;
        model = playerModel.GetChild(0);
        model.transform.localPosition = new(0, -0.891f, 0);
        modelVRM = model.GetComponent<Vrm10Instance>();
        if (model != null)
        {
            modelVRM.UpdateType = Vrm10Instance.UpdateTypes.Update;
        }
    }
}