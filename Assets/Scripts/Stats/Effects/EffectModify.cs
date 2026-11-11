using UnityEngine;

namespace Stats
{
    public interface IEffectModify
    {
        // HashSet<Tags> Tags { get; set; }
        Tags Tag { get; set; }
        int Priority { get; }
        IEffectModify AddTag(Tags tag);
        void Apply(Effect effect);
        bool CanApplyTo(Effect effect);
    }

    public abstract class EffectModify<T> : IEffectModify where T : Effect
    {
        // public HashSet<Tags> Tags { get; set; } = new();
        public Tags Tag { get; set; }
        public virtual int Priority => 0;

        public IEffectModify AddTag(Tags tag)
        {
            // Tags.Add(tag);
            Tag = tag;
            return this;
        }

        public bool CanApplyTo(Effect effect)
        {
            //return effect is T typed && Tags.All(tag => effect.Tags.Contains(tag));
            return effect is T typed && effect.Tags.Contains(Tag);
        }

        public void Apply(Effect effect)
        {
            // Debug.Log(effect.GetType().Name);
            if (effect is T typed)
                ApplyTo(typed);
        }

        public abstract void ApplyTo(T effect);
    }

    public abstract class EffectModifyFloat<T> : EffectModify<EffectFloat> where T : EffectFloat
    {
        public float Value = 1;
        public EffectModifyFloat(float val)
        {
            Value = val;
        }
        public virtual void Log(float val, float FinalValue)
        {
            Debug.Log($"Модификатор {GetType().Name} / {Tag} / {val} / {FinalValue}");
        }
        public override void ApplyTo(EffectFloat effect) { }
    }

    // Умножение (например увеличить урон на 10%)
    public class EffectModifyFloatMultiply : EffectModifyFloat<EffectFloat>
    {
        public override int Priority => 0;
        public EffectModifyFloatMultiply(float val) : base(val) { }
        public override void ApplyTo(EffectFloat effect)
        {
            effect.FinalValue *= Value;
            Log(Value, effect.FinalValue);
        }
    }

    // Сложение (например увеличить урон на 10 единиц)
    public class EffectModifyFloatAdd : EffectModifyFloat<EffectFloat>
    {
        public override int Priority => 1;
        public EffectModifyFloatAdd(float val) : base(val) { }
        public override void ApplyTo(EffectFloat effect)
        {
            effect.FinalValue += Value;
            Log(Value, effect.FinalValue);
        }
    }

    // Иммунитет (применяется к здоровью)
    public class EffectFloatImmune : EffectModifyFloat<EffectHealth>
    {

        public override int Priority => 2;
        public EffectFloatImmune(float val) : base(val) { }
        public override void ApplyTo(EffectFloat effect)
        {
            effect.FinalValue *= Value;
            Log(Value, effect.FinalValue);
        }
    }

    // Устойчивость (применяется к здоровью)
    public class EffectFloatResistance : EffectModifyFloat<EffectHealth>
    {
        public override int Priority => 3;
        public EffectFloatResistance(float val) : base(val) { }
        public override void ApplyTo(EffectFloat effect)
        {
            effect.FinalValue = effect.FinalValue * 100f / (100f + Value);
            Log(Value, effect.FinalValue);
        }
    }

    // перезапись только для определенных шкал (здоровье, мана, выносливость)
    public abstract class EffectModifyFloatOverride<T> : EffectModifyFloat<EffectHealthOverride>
    {
        public EffectModifyFloatOverride(float val) : base(val) { }
        public override void ApplyTo(EffectFloat effect)
        {
            effect.Value = Value;
            Log(Value, effect.FinalValue);
        }
    }

    // перезапись только для здоровья
    public class EffectModifyFloatOverrideHealth : EffectModifyFloat<EffectHealthOverride>
    {
        public EffectModifyFloatOverrideHealth(float val) : base(val) { }
    }
}