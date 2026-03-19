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

        public override object Save()
        {
            EntityComponentScriptData data = new()
            {
                IsExecuted = IsExecuted
            };
            return data;
        }

        public override void Load(object data) 
        {
            EntityComponentScriptData EntityComponentScriptData = (EntityComponentScriptData)data;
            IsExecuted = EntityComponentScriptData.IsExecuted;
        }
    }
}