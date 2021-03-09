public static class IntExtensions
{
    /// <summary>
    /// Checks if value is between lowerbound and upperbound (both inclusive)
    /// </summary>
    public static bool IsBetween(this int value, int lb, int ub) => lb <= value && value <= ub;
}
