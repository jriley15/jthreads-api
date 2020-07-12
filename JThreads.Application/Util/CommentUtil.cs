using JThreads.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Application.Util
{
    public class CommentUtil
    {
        public static int GetTotalReplies(Comment comment)
        {
            var total = 0;
            var stack = new Stack<Comment>();
            stack.Push(comment);

            while (stack.TryPop(out var current))
            {
                total += current.Replies.Count;
                foreach (var reply in current.Replies)
                {
                    stack.Push(reply);
                }
            }
            return total;
        }
    }
}
