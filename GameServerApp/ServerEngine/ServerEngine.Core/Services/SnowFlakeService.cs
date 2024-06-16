
namespace ServerEngine.Core.Services
{
    using Interfaces;

    public class SnowflakeService : ISnowflakeService
    {
        private const long Epoch = 1288834974657L;
        private const int NodeIdBits = 48;
        private const int SequenceBits = 16;
        private const long MaxNodeId = -1L ^ (-1L << NodeIdBits);
        private const long MaxSequence = -1L ^ (-1L << SequenceBits);
        private const int NodeIdShift = SequenceBits;
        private const int TimestampShift = 64;

        private long _lastTimestamp = -1L;
        private long _nodeId;
        private long _sequence = 0L;

        public SnowflakeService()
        {

        }

        public void Initialize(int nodeId, int serverId)
        {
            if (nodeId > MaxNodeId || nodeId < 0)
            {
                throw new ArgumentException($"Node ID must be between 0 and {MaxNodeId}");
            }
            _nodeId = nodeId;
        }

        public string GenerateId()
        {
            lock (this)
            {
                long timestamp = GetCurrentTimestamp();

                if (timestamp < _lastTimestamp)
                {
                    throw new InvalidOperationException("Clock moved backwards. Refusing to generate id for {timestamp - _lastTimestamp} milliseconds");
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & MaxSequence;
                    if (_sequence == 0)
                    {
                        timestamp = WaitNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                long id = ((timestamp - Epoch) << TimestampShift) | (_nodeId << NodeIdShift) | _sequence;
                
                return id.ToString("X16");
            }
        }

        private static long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private long WaitNextMillis(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }
    }
}
