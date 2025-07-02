namespace TestProject.Datas
{
    // ===== 1. 핵심 데이터 구조 (불변 설계) =====
    /// <summary>
    /// Vector3.
    /// </summary>
    public readonly record struct Vector3(float X, float Y, float Z)
    {
        public static Vector3 Zero => new(0, 0, 0);
        public float DistanceTo(Vector3 other) =>
            MathF.Sqrt(MathF.Pow(X - other.X, 2) + MathF.Pow(Y - other.Y, 2) + MathF.Pow(Z - other.Z, 2));
        public Vector3 Add(Vector3 other) => new(X + other.X, Y + other.Y, Z + other.Z);
        public Vector3 Multiply(float scalar) => new(X * scalar, Y * scalar, Z * scalar);
    }

}
