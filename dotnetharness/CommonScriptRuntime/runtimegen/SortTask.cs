using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class SortTask
    {
        public bool isMerged;
        public SortNode left;
        public SortNode right;
        public SortNode mergedHead;
        public SortNode mergedTail;
        public SortTask feedsTo;
        public bool feedsToLeft;
        public SortTask next;

        public SortTask(bool isMerged, SortNode left, SortNode right, SortNode mergedHead, SortNode mergedTail, SortTask feedsTo, bool feedsToLeft, SortTask next)
        {
            this.isMerged = isMerged;
            this.left = left;
            this.right = right;
            this.mergedHead = mergedHead;
            this.mergedTail = mergedTail;
            this.feedsTo = feedsTo;
            this.feedsToLeft = feedsToLeft;
            this.next = next;
        }
    }

}
