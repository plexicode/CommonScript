using CommonScript.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Runtime
{
    public class CanonicalEventLoop
    {
        private RuntimeContext runtimeContext;
        private bool mainStarted = false;
        private List<QueueItem> timedQueue = [];
        private List<RuntimeTask> readyQueue = [];

        private static long NowMillis()
        {
            return DateTime.Now.Ticks / 10000;
        }

        private class QueueItem
        {
            public QueueItem(RuntimeTask task, double time)
            {
                this.Task = task;
                this.ExecutionTimeStamp = time;
            }
            public long ID { get; private set; }
            public RuntimeTask Task { get; private set; }
            public double ExecutionTimeStamp { get; private set; }
        }

        public CanonicalEventLoop(RuntimeContext runtimeContext)
        {
            this.runtimeContext = runtimeContext;
        }

        public TaskResult StartEventLoop()
        {
            if (this.mainStarted) throw new InvalidOperationException("Can only call StartMainTask once.");
            this.mainStarted = true;
            this.QueueTask(this.runtimeContext.MainTask, 0);

            while (this.runtimeContext.HasActiveTasks)
            {
                RuntimeTask nextTask = this.GetNextTaskIfReady();
                if (nextTask != null)
                {
                    TaskResult result = nextTask.Resume();

                    switch (result.Status)
                    {
                        case TaskResultStatus.DONE:
                        case TaskResultStatus.ERROR:
                            return result;

                        case TaskResultStatus.SUSPEND:
                            // Responsibility is delegated...somewhere else.
                            // Note that a suspended task that is never re-invoked will cause the process to linger
                            // as HasActiveTasks will always return true.
                            break;

                        case TaskResultStatus.SLEEP:
                            this.QueueTask(nextTask, FunctionWrapper.PUBLIC_getTaskResultSleepAmount(result) / 1000.0);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    // TODO: Change this to a Tickle-Me Mutex
                    System.Threading.Thread.Sleep(1);
                }
            }

            // In general, this should not happen.
            return new TaskResult(new ExecutionResult(1, null, 0, null, []));
        }

        private RuntimeTask GetNextTaskIfReady()
        {
            if (this.timedQueue.Count > 0)
            {
                long nowMillis = NowMillis();
                List<QueueItem> moveToReady = null;
                for (int i = 0; i < this.timedQueue.Count; i++)
                {
                    QueueItem current = this.timedQueue[i];
                    if (current.ExecutionTimeStamp <= nowMillis)
                    {
                        moveToReady = moveToReady ?? [];
                        moveToReady.Add(current);
                        this.timedQueue.RemoveAt(i--);
                    }
                }

                if (moveToReady != null)
                {
                    if (moveToReady.Count > 1)
                    {
                        moveToReady = [.. moveToReady.OrderBy(v => v.ExecutionTimeStamp)];
                    }
                    this.readyQueue.AddRange(moveToReady.Select(v => v.Task));
                }
            }

            if (this.readyQueue.Count > 0)
            {
                RuntimeTask t = this.readyQueue[0];
                this.readyQueue.RemoveAt(0);
                return t;
            }

            return null;
        }

        public void QueueTask(RuntimeTask task, double delaySeconds)
        {
            if (delaySeconds <= 0)
            {
                this.readyQueue.Add(task);
            }
            else
            {
                long execDate = NowMillis() + (long)(delaySeconds * 1000 + 0.5);
                this.timedQueue.Add(new QueueItem(task, execDate));
            }
        }
    }
}
