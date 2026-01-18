using System.Collections.Generic;

namespace Stats
{
    public enum Tags
    {
        // Урон
        Damage,
        Damage_Phyical,
        Damage_Fire,
        Damage_Ice,
        Damage_Electric,
        Damage_Toxic,
        Damage_Radiation,

        // Эффекты (баффы)
        Fire,
        Ice,
        Electric,
        Toxic,
        Radiation,

        Sleep,
        Disorientation,
        Stun,
        Slowing,
        Heal,

        // Прочее
        Healing,
        Magic,
        Phyical,
        Mental,
        Push,
        Fall,

        Death,
        Lava,
        Deadly_Fall,
        Deadly_Poison,
        Strangulation,
        Dismemberment,
        Burn,
        Thirst,
        Blood_Loss,
    }

    // public class Tag
    // {
    //     public virtual string Name { get; }
    //     public virtual List<Tag> Tags { get; }
    // }

    // class TagDamage : Tag
    // {
    //     public override string Name => "Урон";
    //     public TagDamage()
    //     {
    //         Tags.Add(new TagDamagePhyical());
    //     }
    // }

    // class TagDamagePhyical : Tag
    // {
    //     public override string Name => "Физический";
    //     public TagDamagePhyical()
    //     {
    //         // Tags.Add();
    //     }
    // }
}