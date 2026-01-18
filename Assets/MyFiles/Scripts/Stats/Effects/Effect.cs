using System.Collections.Generic;
using System.Linq;
using Entities_1;
using UnityEngine;


namespace Stats
{
    public class Effect
    {
        public HashSet<Tags> Tags { get; } = new();
        public List<IEffectModify> Modifiers { get; } = new();

        public Effect AddTag(Tags tag)
        {
            Tags.Add(tag);
            return this;
        }

        public Effect AddTags(params Tags[] tags)
        {
            foreach (var tag in tags)
                Tags.Add(tag);
            return this;
        }

        public virtual void AddModifiers(List<IEffectModify> modifiers)
        {
            foreach (var mod in modifiers)
            {
                if (mod.CanApplyTo(this))
                {
                    Modifiers.Add(mod);
                }
            }
        }

        public virtual void Apply(BaseEntity entity) { }
    }


    public class EffectFloat : Effect
    {
        public float Value = 0;
        public float FinalValue = 0;
        public EffectFloat(float value) => Value = value;
        public override void Apply(BaseEntity entity)
        {
            base.Apply(entity);
            FinalValue = Value;
            entity.NowStats.Health.ChangeNowValue(FinalValue);
            Debug.Log($"Изменение шкалы {FinalValue} / {entity.name}");
        }
    }

    public class EffectHealth : EffectFloat
    {
        public EffectHealth(float value) : base(value) { }

        public float CalcValue()
        {
            FinalValue = Value;
            foreach (var mod in Modifiers.OrderBy(m => m.Priority))
            {
                mod.Apply(this);
                // Debug.Log($"Модификатор применен {mod.GetType().Name} / {FinalValue}");
            }
            return FinalValue;
        }

        public override void Apply(BaseEntity entity)
        {
            FinalValue = CalcValue();
            entity.NowStats.Health.ChangeNowValue(FinalValue);
            Debug.Log($"Изменение здоровья {FinalValue} / {entity.name}");
        }
    }

    public class EffectHealthOverride : EffectHealth
    {
        public EffectHealthOverride(float value) : base(value) { }
        public override void Apply(BaseEntity entity)
        {
            FinalValue = Value;
            entity.NowStats.Health.OverrideNowValue(FinalValue);
            Debug.Log($"Перезапись здоровья {FinalValue} / {entity.name}");
        }
    }

    public class EffectDamage : EffectFloat { public EffectDamage(float value) : base(value) { AddTag(Stats.Tags.Damage); } }
    public class EffectDamageFire : EffectDamage { public EffectDamageFire(float value) : base(value) { AddTag(Stats.Tags.Damage_Fire); } }

    public class EffectBuff : Effect
    {
        public float Time;

        public EffectBuff(float time) { Time = time; }

        public override void Apply(BaseEntity entity)
        {
            Debug.Log($"Бафф {Time} / {entity.name}");
        }
    }

    public class EffectPush : Effect
    {
        public float Value;
        public Vector3 Vector;
        public EffectPush(float value, Vector3 vector)
        {
            Value = value;
            Vector = vector;
            AddTag(Stats.Tags.Push);
        }
        public override void Apply(BaseEntity entity)
        {
            Debug.Log($"Отталкивание {Value} / {entity.name}");
        }
    }
}