using System;
using System.Collections.Generic;

namespace AspNetCoreMiddleware.Algorithm
{
    public class RateLimitAlgorithm
    {
        /// <summary>
        /// Check if new requests can be executed immediately or need to wait N seconds
        /// </summary>
        /// <param name="sec">Allowed amount or requests per second</param>
        /// <param name="count">Allowed amount of requests</param>
        /// <param name="now">DateTime of request</param>
        /// <param name="queue">Queue of requests</param>
        /// <returns>Amount of seconds need to wait until request can be executed.
        /// 0 - Can be executed immediately
        /// >0 - Amount of seconds to wait
        /// </returns>
        public int RequestAllowedInSec(int sec, int count, DateTime now, ref Queue<DateTime> queue)
        {
            int result = 0;

            // If queue is empty - create new queue and add first request that come, no limits check
            if (queue == null)
            {
                queue = new Queue<DateTime>();

                queue.Enqueue(now);

                result = 0;
            }
            // If there is some requests times in a queue - remove expired
            else
            {
                DateTime firstDateTime = queue.Peek();

                // Remove expired one (Check if requests is expired and remove from queue)
                while (queue.Count > 0 && firstDateTime.AddSeconds(sec) < now)
                {
                    queue.Dequeue();

                    if (queue.Count > 0)
                        firstDateTime = queue.Peek();
                    else
                        break;
                }

                // Check for count according to the rules
                if (queue.Count < count)
                {
                    // Add request to queue only if it's allowed by limits
                    queue.Enqueue(now);
                    result = 0;
                }
                else
                {
                    if (queue.Count > 0)
                    {
                        firstDateTime = queue.Peek();

                        // Calculate the remaining seconds (Now - First request in queue)
                        result = sec - Convert.ToInt32((now - firstDateTime).TotalSeconds);
                    }
                }
            }

            return result;
        }
    }
}