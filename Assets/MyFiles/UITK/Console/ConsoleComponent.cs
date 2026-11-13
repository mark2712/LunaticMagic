using System;
using UnityEngine.UIElements;

namespace UITK
{
    public class ConsoleComponent : UIComponent<Type>
    {
        public ConsoleComponent(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
        }

        public override void Render()
        {
        }
    }
}
