using States;

public static class PlayerSpeed
{
    public static float Base = 3.5f;
    public static float Rope;
    public static float Run;
    public static float Sneak;
    public static float SneakSlow;
    public static float Hit;

    static PlayerSpeed()
    {
        Rope = Base * 0.2f;
        Run = Base * 2f;
        Sneak = Base * 0.62f;
        SneakSlow = Base * 0.42f;
        Hit = Base * 0.12f;
    }

    public static void Update()
    {
        GameContext.PlayerController.NowMoveSpeed = Get();
    }

    public static float Get()
    {
        if (Flags.Get(FlagName.LegsRope))
        {
            return Rope;
        }
        else
        {
            if (Flags.Get(FlagName.Hit))
            {
                return Hit;
            }
            else
            {
                if (Flags.Get(FlagName.Sneak))
                {
                    if (Flags.Get(FlagName.Shift))
                    {
                        return Sneak;
                    }
                    else
                    {
                        return SneakSlow;
                    }
                }
                else
                {
                    if (Flags.Get(FlagName.Shift))
                    {
                        return Run;
                    }
                    else
                    {
                        return Base;
                    }
                }
            }
        }
    }
}