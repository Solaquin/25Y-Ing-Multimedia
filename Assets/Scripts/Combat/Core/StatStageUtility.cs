public static class StatStageUtility
{
    public static float GetMultiplier(int stage)
    {
        if (stage >= 0)
            return (2f + stage) / 2f;

        return 2f / (2f - stage);
    }
}