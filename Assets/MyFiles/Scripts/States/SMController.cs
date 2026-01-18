using System.Collections.Generic;

namespace States
{
    public class SMController
    {
        public MainSM MainSM;
        public ModalSM ModalSM;
        public LegsSM LegsSM;
        public HandsSM HandsSM;
        public HandRightSM HandRightSM;
        public HandLeftSM HandLeftSM;
        public readonly List<SM> allSM = new();

        public SMController()
        {
            MainSM = new();
            Register(MainSM);

            ModalSM = new();
            Register(ModalSM);

            LegsSM = new();
            Register(LegsSM);

            HandsSM = new();
            Register(HandsSM);
        }

        public void Initialize(Dictionary<SM, State> initialStates = null)
        {
            // можно передать начальные состояния в initialStates для нужных SM. Если начальное состояние не указано то будет выбрано DefaultState
            foreach (var sm in allSM)
            {
                State state = initialStates != null && initialStates.TryGetValue(sm, out var initState)
                    ? initState
                    : sm.DefaultState;

                sm.GoToStateEnter(state);
            }
        }

        private void Register(SM sm)
        {
            sm.smController = this;
            allSM.Add(sm);
        }

        public void FixedUpdate()
        {
            foreach (var sm in allSM)
            {
                sm.FixedUpdate();
            }
        }

        public void Update()
        {
            foreach (var sm in allSM)
            {
                sm.Update();
            }
        }

        public void PauseUpdate()
        {
            MainSM.Update();
        }

        public void LateUpdate()
        {
            foreach (var sm in allSM)
            {
                sm.LateUpdate();
            }
        }

        public void PauseLateUpdate()
        {
            MainSM.LateUpdate();
        }
    }
}