public static class RPGProgress
{
    private static bool[] metersActivated = new bool[3];
    public static bool IsMeterActivated(int index) => index >= 0 && index < 3 && metersActivated[index];
    public static void ActivateMeter(int index) { if (index >= 0 && index < 3) metersActivated[index] = true; }
    public static void Reset() => metersActivated = new bool[3];
}