using UnityEngine;

namespace States
{
    public class PauseBase : State
    {
        private float _time;
        public override void Enter()
        {
            GameContext.IsPause = true;
            _time = Time.timeScale;
            Time.timeScale = 0;
            Cursor.visible = true;
        }

        public override void Exit()
        {
            GameContext.IsPause = false;
            Time.timeScale = _time;
            Cursor.visible = false;
        }
    }
}