﻿
namespace ServerEngine.Core.Services
{
	using ServerEngine.Core.Services.Interfaces;
	using ServerEngine.Core.Util;

	/// <summary>
	/// SnowflakeService.
	/// Unique id generator service.
	/// </summary>
	public sealed class SnowflakeService : IUniqueIdService
    {
        // EpochBaseTime: 특정 시간 이후로 몇 밀리초가 경과했는지를 나타내는 값.
        private readonly DateTime _epochBaseTime;
        private long _epoch = 0;

        // Bits number.
        private const int DatacenterBits = 24;
        private const int WorkerBits = 24;
        private const int SequenceBits = 16;

        // Max value.
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterBits);
        private const long MaxWorkerId = -1L ^ (-1L << WorkerBits);
        private const long MaxSequence = -1L ^ (-1L << SequenceBits);

        // bit shift.
        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerBits;
        private const int TimestampShift = SequenceBits + WorkerBits + DatacenterBits;

        // Values.
        private long _lastTimestamp = -1L;
        private long _dcId;
        private long _workerId;
        private long _sequence = 0L;

        public SnowflakeService()
        {
            _epochBaseTime = new DateTime(2024, 1, 1, 0, 0, 0).ToUniversalTime();
        }

        /// <summary>
        /// Initialize configure datas.
        /// </summary>
        public void Initialize(DateTime baseTime, int dcId, int workerId)
        {
            // Initialize configure datas.
            if (dcId > MaxDatacenterId || dcId < 0)
            {
                return;
            }

            if (workerId > MaxWorkerId || workerId < 0)
            {
                return;
            }

            if (true == TimeUtil.IsPastTime(_epochBaseTime, baseTime))
            {
                return;
            }

            // Set configure datas.
            var epochTime = baseTime.ToUniversalTime() - _epochBaseTime;
            _epoch = (long)epochTime.TotalMilliseconds;
            _dcId = dcId;
            _workerId = workerId;
        }

        /// <summary>
        /// Generate unique id.
        /// </summary>
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
                
                // ID.
                // Position: (MSB)Time | DataCenter | WorkerId | Sequence.
                var id = ((timestamp - _epoch) << TimestampShift) | (_dcId << DatacenterIdShift) | (_workerId << WorkerIdShift) | _sequence;
                
                return id.ToString("X16");
            }
        }

        /// <summary>
        /// Get current time in utc milli seconds.
        /// </summary>
        private static long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Get next milli seconds time.
        /// </summary>
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
