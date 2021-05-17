using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Core.Domain;

namespace Bifrost.Connector.Web.Helpers
{
    public static class QueueItemExtensions
    {
        private const string Delimiter = "<|>";


        public static QueueItem ToQueueItem(this Uri uri, Page sourcePage)
        {
            return new QueueItem(uri.ToString())
            {
                OptionalData = $"{sourcePage.Depth+1}{Delimiter}"
            };
        }


        public static IList<PageQueueItem> ToPageQueueItems(this IEnumerable<QueueItem> queueItems)
        {
            return queueItems.Select(q => q.ToPageQueueItem()).ToList();
        }
        public static IList<QueueItem> ToQueueItems(this IEnumerable<PageQueueItem> pageQueueItems)
        {
            return pageQueueItems.Select(q => q.ToQueueItem()).ToList();
        }
        public static PageQueueItem ToPageQueueItem(this QueueItem queueItem)
        {
            var tokens = queueItem.OptionalData.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            var depth = 0;
            int.TryParse(tokens.FirstOrDefault(), out depth);
            var url = tokens.Length == 2 ? new Uri(tokens[1]) : new Uri(queueItem.DocumentId);
            return new PageQueueItem()
            {
                Id = queueItem.DocumentId,
                CreateDate = queueItem.CreateDate,
                Depth = depth,
                Url = url
            };
        }

        public static QueueItem ToQueueItem(this PageQueueItem pageQueueItem)
        {
            return new QueueItem()
            {
                DocumentId = pageQueueItem.Id,
                CreateDate = pageQueueItem.CreateDate,
                OptionalData = $"{pageQueueItem.Depth}{Delimiter}{pageQueueItem.Url}"
            };
        }
    }


}