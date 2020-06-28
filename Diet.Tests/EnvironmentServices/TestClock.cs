using System;
using Diet.Api.Infrastructure;

namespace Diet.Tests.EnvironmentServices
{
    /// <summary>
    /// This should be used to manage date and time operations within the application
    /// This will be used for integration tests
    /// </summary>
    public class TestClock : IClock
    {
        private DateTime? _utcNow;

        public DateTime UtcNow
        {
            get
            {
                if (!_utcNow.HasValue) return DateTime.UtcNow;
                
                var backup = _utcNow;
                _utcNow = null;
                return backup.Value;
            }
        }

        public void Set(DateTime? value)
        {
            _utcNow = value;
        }
    }
}