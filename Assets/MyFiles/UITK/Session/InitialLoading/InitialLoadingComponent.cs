using System;
using UnityEngine.UIElements;

namespace UITK
{
    public class InitialLoadingComponent : UIComponent<Type>
    {
        public InitialLoadingComponent(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
        }

        public override void Render()
        {
        }
    }
}
