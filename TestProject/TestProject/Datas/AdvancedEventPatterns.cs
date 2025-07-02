using System.Reactive.Linq;

namespace TestProject.Datas
{
    // ===== 10. 고급 이벤트 패턴 =====
    /// <summary>
    /// AdvancedEventPatterns.
    /// </summary>
    public class AdvancedEventPatterns
    {
        private readonly EventStreamManager _eventManager;

        public AdvancedEventPatterns(EventStreamManager eventManager)
        {
            _eventManager = eventManager;
            SetupPatterns();
        }

        private void SetupPatterns()
        {
            // 1. 이벤트 시간 윈도우 분석
            _eventManager.GetEventStream<PlayerMoveEvent>()
                .Window(TimeSpan.FromSeconds(5))
                .Subscribe(window =>
                {
                    window.Count().Subscribe(count =>
                    {
                        if (count > 1000) // 5초에 1000번 이상 이동
                        {
                            Console.WriteLine($"🔥 High activity detected: {count} moves in 5 seconds");
                        }
                    });
                });

            // 2. 플레이어 행동 패턴 분석
            _eventManager.GetEventStream<PlayerMoveEvent>()
                .GroupBy(evt => evt.PlayerId)
                .Subscribe(playerGroup =>
                {
                    playerGroup
                        .Buffer(10) // 10번의 이동을 버퍼링
                        .Subscribe(moves =>
                        {
                            if (AnalyzeMovementPattern(moves))
                            {
                                Console.WriteLine($"🤖 Bot-like behavior detected: Player {playerGroup.Key}");
                            }
                        });
                });

            // 3. 섹션별 활동 모니터링
            _eventManager.GetEventStream<SectionChangeEvent>()
                .GroupBy(evt => evt.To)
                .Subscribe(sectionGroup =>
                {
                    sectionGroup
                        .Buffer(TimeSpan.FromSeconds(10))
                        .Subscribe(changes =>
                        {
                            if (changes.Count() > 50) // 10초에 50명 이상 진입
                            {
                                Console.WriteLine($"🎯 Popular section: {sectionGroup.Key} ({changes.Count()} entries)");
                            }
                        });
                });
        }

        private bool AnalyzeMovementPattern(IList<PlayerMoveEvent> moves)
        {
            if (moves.Count < 5) return false;

            // 너무 규칙적인 이동 패턴 감지
            var intervals = moves.Zip(moves.Skip(1), (a, b) => (b.Timestamp - a.Timestamp).TotalMilliseconds).ToArray();
            var avgInterval = intervals.Average();
            var deviation = intervals.Select(i => Math.Abs(i - avgInterval)).Average();

            return deviation < 10; // 10ms 이하의 편차 = 의심스러운 봇
        }
    }

}
