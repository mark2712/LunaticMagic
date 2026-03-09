using System;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public class EntityComponentScriptData
    {
        public bool IsExecuted = false;
    }

    public abstract class EntityComponentScript : EntityComponentBase
    {
        private bool IsExecuted;

        public override void Start()
        {
            if (IsExecuted) return;

            Execute();
            IsExecuted = true;
        }

        protected abstract void Execute();

        public override string Save()
        {
            EntityComponentScriptData data = new()
            {
                IsExecuted = IsExecuted
            };
            return SaveData.SaveStr(data);
        }

        public override void Load(string data)
        {
            IsExecuted = SaveData.LoadStr<EntityComponentScriptData>(data).IsExecuted;
        }
    }


    public class TestScriptComponent : EntityComponentScript
    {
        protected override void Execute()
        {
            Debug.Log("Script executed once");
        }
    }
}