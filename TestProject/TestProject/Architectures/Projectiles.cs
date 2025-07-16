using System.Numerics;

namespace TestProject.Architectures
{
    /// <summary>
    /// 서버 권위 방식의 발사체 클래스
    /// </summary>
    public class Projectile
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Gravity { get; set; } = -9.81f * 2f; // 시각적 효과를 위해 2배 중력
        public float Damage { get; set; }
        public float Radius { get; set; } // 충돌 반경
        public float LifeTime { get; set; }
        public float MaxRange { get; set; }
        public Vector3 StartPosition { get; set; }
        public bool IsActive { get; set; } = true;

        // 충돌 관련
        public HashSet<int> HitEntities { get; set; } = new HashSet<int>();
        public bool IsPiercing { get; set; }
        public int MaxPierceCount { get; set; } = 1;
        public float DamageReductionPerPierce { get; set; } = 0.2f;

        public Projectile(Vector3 startPos, Vector3 velocity, float damage, float radius)
        {
            StartPosition = startPos;
            Position = startPos;
            Velocity = velocity;
            Damage = damage;
            Radius = radius;
        }

        /// <summary>
        /// 발사체 업데이트 (서버 틱마다 호출)
        /// </summary>
        public void Update(float deltaTime)
        {
            // 이전 위치 저장 (연속 충돌 감지용)
            Vector3 previousPosition = Position;

            // 탄도 물리 적용
            Velocity = new Vector3(
                Velocity.X,
                Velocity.Y + Gravity * deltaTime,
                Velocity.Z
            );

            Position += Velocity * deltaTime;
            LifeTime += deltaTime;

            // 최대 사거리 또는 지면 충돌 체크
            float distanceTraveled = Vector3.Distance(StartPosition, Position);

            if (distanceTraveled > MaxRange || Position.Y < 0 || LifeTime > 10f)
            {
                IsActive = false;
            }
        }
    }

    /// <summary>
    /// 발사체 궤적 계산 유틸리티
    /// Trajectory: 궤도.
    /// </summary>
    public static class ProjectileTrajectory
    {
        /// <summary>
        /// 목표 지점을 맞추기 위한 발사 각도 계산
        /// </summary>
        /// <returns>저각(lowAngle)과 고각(highAngle) 반환, 도달 불가능시 null</returns>
        public static (float? lowAngle, float? highAngle) CalculateLaunchAngle(
            Vector3 origin, Vector3 target, float launchSpeed, float gravity = 9.81f)
        {
            Vector3 targetVec = target - origin;
            float y = targetVec.Y;
            targetVec.Y = 0;

            // target - origin 사이의 거리.
            float x = targetVec.Length();

            float g = Math.Abs(gravity);
            float vSqr = launchSpeed * launchSpeed;
            float underRoot = vSqr * vSqr - g * (g * x * x + 2 * y * vSqr);

            if (underRoot < 0)
            {
                // 목표 지점이 사거리 밖
                return (null, null);
            }

            float sqrtValue = (float)Math.Sqrt(underRoot);
            float highAngle = (float)Math.Atan2(vSqr + sqrtValue, g * x);
            float lowAngle = (float)Math.Atan2(vSqr - sqrtValue, g * x);

            return (lowAngle, highAngle);
        }

        /// <summary>
        /// 발사 속도 벡터 계산
        /// </summary>
        public static Vector3 CalculateLaunchVelocity(
            Vector3 origin, Vector3 target, float angle, float launchSpeed)
        {
            Vector3 direction = target - origin;
            direction.Y = 0;
            direction = Vector3.Normalize(direction);

            float vx = launchSpeed * (float)Math.Cos(angle);
            float vy = launchSpeed * (float)Math.Sin(angle);

            return new Vector3(
                direction.X * vx,
                vy,
                direction.Z * vx
            );
        }

        /// <summary>
        /// 궤적 예측 포인트 생성 (시각화용)
        /// </summary>
        public static List<Vector3> PredictTrajectory(
            Vector3 startPos, Vector3 velocity, float gravity,
            float timeStep = 0.1f, int maxSteps = 100)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 pos = startPos;
            Vector3 vel = velocity;

            for (int i = 0; i < maxSteps; i++)
            {
                points.Add(pos);

                vel.Y += gravity * timeStep;
                pos += vel * timeStep;

                if (pos.Y < 0) break; // 지면 도달
            }

            return points;
        }
    }
}