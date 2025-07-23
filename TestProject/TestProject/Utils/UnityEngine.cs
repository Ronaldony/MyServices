namespace TestProject.Utils
{

    // Placeholder for Unity's Random (replace with System.Random in non-Unity environment)
    public static class UnityEngine
    {
        public static class Random
        {
            public static float value => NextDouble();
            public static float Range(float min, float max) => min + (max - min) * NextDouble();
            public static float NextDouble() => 1.0f;
        }
    }
}
