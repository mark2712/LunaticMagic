using System;
using UnityEngine.UIElements;

namespace UITK
{
    public class GameScreenComponent : UIComponent<Type>
    {
        public GameScreenComponent(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
        }

        public override void Render()
        {
        }
    }
}
