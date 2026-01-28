using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class SaveButtonComponent : UIComponent<Type>
    {
        public SaveButtonComponent(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            var SaveButton = View.Q<Button>("SaveButton");
            RegisterCallback<ClickEvent>(SaveButton, _ => OnSave());
        }

        private void OnSave()
        {
            GlobalGame.Session.CreateSave(GameSaveTypes.Quick, "");
        }
    }
}