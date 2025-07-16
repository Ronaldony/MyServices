using System.Numerics;
using TestProject.Datas;

namespace TestProject.Architectures
{
    /// <summary>
    /// 발사체 네트워크 동기화 관리
    /// </summary>
    public class ProjectileNetworkSync
    {
        private readonly ProjectileManager projectileManager;
        private readonly Dictionary<int, ProjectileSnapshot> lastSnapshots;

        private readonly float snapshotInterval = 0.05f; // 50ms 간격
        private float lastSnapshotTime;
        private readonly IGameWorld GameWorld;
        private readonly IGameData GameData;

        public ProjectileNetworkSync(ProjectileManager manager)
        {
            projectileManager = manager;
            lastSnapshots = new Dictionary<int, ProjectileSnapshot>();
        }

        /// <summary>
        /// 클라이언트 발사 요청 처리
        /// </summary>
        public void HandleClientFireRequest(int playerId, FireProjectilePacket packet)
        {
            // 서버 측 검증
            if (!ValidateFireRequest(playerId, packet))
            {
                // 유효하지 않은 발사 요청.
                SendFireDenied(playerId, "Invalid fire request");
                return;
            }

            // 발사체 생성
            var projectileData = new ProjectileData
            {
                OwnerId = playerId,
                Origin = packet.Origin,
                Target = packet.Target,
                LaunchSpeed = packet.LaunchSpeed,
                Damage = CalculateDamage(playerId, packet.WeaponId),
                Radius = packet.ProjectileRadius,
                MaxRange = packet.MaxRange,
                UseHighAngle = packet.UseHighAngle,
                IsPiercing = packet.IsPiercing
            };

            // 
            int projectileId = projectileManager.CreateProjectile(projectileData);

            if (projectileId > 0)
            {
                // 발사 확인 패킷 전송
                SendFireConfirmed(playerId, projectileId, projectileData);

                // 다른 플레이어들에게 브로드캐스트
                BroadcastProjectileCreated(projectileId, projectileData);
            }
        }

        /// <summary>
        /// 주기적 스냅샷 생성 및 전송
        /// </summary>
        public void UpdateSnapshots(float currentTime)
        {
            if (currentTime - lastSnapshotTime < snapshotInterval)
                return;

            lastSnapshotTime = currentTime;

            var snapshots = new List<ProjectileSnapshot>();

            foreach (var kvp in projectileManager.ActiveProjectiles)
            {
                var projectile = kvp.Value;
                var snapshot = new ProjectileSnapshot
                {
                    Id = projectile.Id,
                    Position = projectile.Position,
                    Velocity = projectile.Velocity,
                    Timestamp = currentTime
                };

                // 델타 압축을 위한 이전 스냅샷과 비교
                if (lastSnapshots.TryGetValue(projectile.Id, out var lastSnapshot))
                {
                    // 변화가 있는 경우만 전송
                    if (Vector3.Distance(snapshot.Position, lastSnapshot.Position) > 0.1f)
                    {
                        snapshots.Add(snapshot);
                        lastSnapshots[projectile.Id] = snapshot;
                    }
                }
                else
                {
                    snapshots.Add(snapshot);
                    lastSnapshots[projectile.Id] = snapshot;
                }
            }

            if (snapshots.Count > 0)
            {
                BroadcastSnapshots(snapshots);
            }
        }

        /// <summary>
        /// 클라이언트 예측 조정
        /// </summary>
        public void HandleClientPredictionCheck(int playerId, int projectileId, Vector3 predictedPos)
        {
            bool isValid = projectileManager.ValidateClientPrediction(
                projectileId, predictedPos, tolerance: 2f);

            if (!isValid)
            {
                // 서버 권위 위치로 강제 동기화
                ForceProjectileSync(playerId, projectileId);
            }
        }

        private bool ValidateFireRequest(int playerId, FireProjectilePacket packet)
        {
            // 플레이어 상태 검증
            var player = GameWorld.GetPlayer(playerId);
            if (player == null || !player.IsAlive)
                return false;

            // 쿨다운 검증
            if (!player.CanFire(packet.WeaponId))
                return false;

            // 탄약 검증
            if (!player.HasAmmo(packet.WeaponId))
                return false;

            // 위치 검증 (치트 방지)
            float distance = Vector3.Distance(player.Position, packet.Origin);
            if (distance > 5f) // 허용 오차
                return false;

            return true;
        }

        private float CalculateDamage(int playerId, int weaponId)
        {
            // 플레이어 스탯과 무기 정보를 기반으로 데미지 계산
            var player = GameWorld.GetPlayer(playerId);
            var weapon = GameData.GetWeapon(weaponId);

            return weapon.BaseDamage * player.DamageMultiplier;
        }

        // 네트워크 전송 메서드들
        private void SendFireConfirmed(int playerId, int projectileId, ProjectileData data) { }
        private void SendFireDenied(int playerId, string reason) { }
        private void BroadcastProjectileCreated(int projectileId, ProjectileData data) { }
        private void BroadcastSnapshots(List<ProjectileSnapshot> snapshots) { }
        private void ForceProjectileSync(int playerId, int projectileId) { }
    }

    /// <summary>
    /// 발사체 스냅샷
    /// </summary>
    public struct ProjectileSnapshot
    {
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Timestamp { get; set; }
    }

    /// <summary>
    /// 발사 요청 패킷
    /// </summary>
    public struct FireProjectilePacket
    {
        public int WeaponId { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 Target { get; set; }
        public float LaunchSpeed { get; set; }
        public float ProjectileRadius { get; set; }
        public float MaxRange { get; set; }
        public bool UseHighAngle { get; set; }
        public bool IsPiercing { get; set; }
    }
}