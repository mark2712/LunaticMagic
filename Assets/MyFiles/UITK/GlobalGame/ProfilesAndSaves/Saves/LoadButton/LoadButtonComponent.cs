using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class LoadButtonComponent : UIComponent<GameSaveLineProps>
    {
        public LoadButtonComponent(GameSaveLineProps props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            var btn = View.Q<Button>("LoadGame");
            RegisterCallback<ClickEvent>(btn, _ =>
            {
                if (Props?.Profile != null && Props.Save != null)
                {
                    GlobalGame.LoadGameSession(Props.Profile, Props.Save);
                }
            });
        }
    }
}
