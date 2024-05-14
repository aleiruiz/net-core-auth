namespace XLocker.Helpers
{
    public static class GenerateRandomNumber
    {
        public static string GetRandomNumber(int digits)
        {
            Random generator = new Random();
            return generator.Next(0, (int)Math.Pow(10, digits)).ToString("D6");
        }
    }
}
