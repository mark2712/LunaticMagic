using UnityEngine;
using Entities_1;
using Stats;
using System.Collections.Generic;
using System.Linq;

public class TestMagic : MonoBehaviour
{
    public BaseEntity CreatorEntity; // UnknownEntity;
    private void OnTriggerEnter(Collider other)
    {
        BaseEntity entity = other.GetComponent<BaseEntity>(); // Получаем сущность попавшую под действие заклинания

        if (entity != null) // Если это сущность
        {
            // модификаторы от заклинателя
            List<IEffectModify> modifiersCreatorEntity = new()
            {
                new EffectModifyFloatAdd(-10).AddTag(Tags.Damage_Ice),
                new EffectModifyFloatMultiply(1.5f).AddTag(Tags.Damage),
            };

            // модификаторы от сущности
            List<IEffectModify> modifiersEntity = new()
            {
                new EffectFloatResistance(15).AddTag(Tags.Damage_Fire),
                new EffectFloatImmune(0.2f).AddTag(Tags.Damage_Ice)
            };

            // различные эффекты
            List<Effect> effects = new();

            Effect effect1 = new EffectHealth(-30);
            effect1.AddTags(Tags.Damage, Tags.Damage_Ice);
            effects.Add(effect1);

            Effect effect3 = new EffectHealth(-20);
            effect3.AddTags(Tags.Damage, Tags.Damage_Fire).AddTags(Tags.Magic);
            effects.Add(effect3);

            Effect effect2 = new EffectBuff(10);
            effect2.AddTags(Tags.Fire);
            effects.Add(effect2);

            // применяем эффекты заклинания и модификаторы
            foreach (var effect in effects)
            {
                effect.AddModifiers(modifiersCreatorEntity);
                effect.AddModifiers(modifiersEntity);
                effect.Apply(entity);
            }
        }
    }


}

public class Predmet
{
    public List<Effect> effects = new();
    public List<IEffectModify> modifiers = new();

    public Predmet()
    {
        // effects.Add(CreateEffectHealth(-30));
        // effects.Add(CreateEffectBuff(10));

        Effect effect1 = new EffectHealth(-30);
        effect1.AddTags(Tags.Damage, Tags.Damage_Ice);
        effects.Add(effect1);

        Effect effect2 = new EffectBuff(10);
        effect2.AddTags(Tags.Fire);
        effects.Add(effect2);

        IEffectModify mod1 = new EffectFloatResistance(10);
        mod1.AddTag(Tags.Damage_Fire);
        modifiers.Add(mod1);

        IEffectModify mod2 = new EffectFloatImmune(0.5f);
        mod1.AddTag(Tags.Fire);
        modifiers.Add(mod2);
    }

    // public Effect CreateEffectHealth(float value)
    // {
    //     Effect effect = new EffectHealth(value);
    //     effect.AddTags(Tags.Damage, Tags.Damage_Ice);
    //     return effect;
    // }
    // public Effect CreateEffectBuff(float value)
    // {
    //     Effect effect = new EffectBuff(value);
    //     effect.AddTags(Tags.Fire);
    //     return effect;
    // }
}