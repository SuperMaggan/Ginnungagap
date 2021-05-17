using System;

namespace Bifrost.Common.Core.ApplicationServices.Implementations
{
    public class TimedCacheItem<TValue>
    {
        public TimedCacheItem(TValue item, DateTime expirationDate)
        {
            Item = item;
            ExpirationDate = expirationDate;
        }

        public TValue Item { get; set; }
        public DateTime ExpirationDate { get; set; }

        public bool HasExpired()
        {
            return DateTime.Now > ExpirationDate;
        }
    }
}