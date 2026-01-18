using UnityEngine;

namespace States
{
    public class BaseAir : BaseGame
    {
        public BaseAir() : base()
        {
            // RegisterEvent(StateEvent.MyEvent, (state, i) =>
            // {
            //     Debug.Log("MyEvent in BaseAir");
            //     InvokeEvent(StateEvent.MyEvent, i - 1);
            //     return null;
            // });
        }
    }
}