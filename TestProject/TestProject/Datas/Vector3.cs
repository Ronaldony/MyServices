namespace TestProject.Datas
{
    // Immutable 데이터 구조
    public readonly struct Vector3
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public float DistanceTo(Vector3 other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            var dz = Z - other.Z;
            return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public Vector3 Add(Vector3 other) => new(X + other.X, Y + other.Y, Z + other.Z);
        public Vector3 Subtract(Vector3 other) => new(X - other.X, Y - other.Y, Z - other.Z);
        public Vector3 Multiply(float scalar) => new(X * scalar, Y * scalar, Z * scalar);
    }
}
