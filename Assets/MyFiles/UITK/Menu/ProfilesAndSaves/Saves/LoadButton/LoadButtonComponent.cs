using System;
using System.Linq;
using UnityEngine.UIElements;

namespace UITK
{
    // public class LoadButtonComponentProps
    // {
    //     public string ProfileId;
    //     public string SaveId;
    // }

    public sealed class LoadButtonComponent : UIComponent<GameSave>
    {
        public LoadButtonComponent(GameSave props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            var btn = View.Q<Button>("LoadGame");
            RegisterCallback<ClickEvent>(btn, _ =>
            {
                GlobalGame.ChangeSession(
                    UIGlobalState.Profiles.Profiles[
                        UIGlobalState.Profiles.GameProfilesSaves.First(p => p.Value.Saves.ContainsKey(Props.SaveId.Value)).Key
                    ],
                    Props
                );
            });
        }
    }
}