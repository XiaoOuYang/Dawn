using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces
{
    /// <summary>
    /// 循环队列
    /// </summary>
    public class CircularQueue
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static CircularQueue Instance = new CircularQueue(60);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slotNum">插槽的数量</param>
        private CircularQueue(int slotNum)
        {
            _slotNum = slotNum;
            _queues = new List<IList<TaskContent>>(_slotNum);

            for (int i = 0; i < _slotNum; i++)
                _queues.Add(new List<TaskContent>());

            _timer = new System.Timers.Timer(1000);
        }

        /// <summary>
        /// 当前下标
        /// </summary>
        private int _currentIndex = 0;
        /// <summary>
        /// 插槽的数量
        /// </summary>
        private int _slotNum = 60;

        private IList<IList<TaskContent>> _queues;

        private bool _started = false;
        private System.Timers.Timer _timer;

        public void Start()
        {
            if (_started == false)
            {
                _timer.Enabled = true;
                _timer.Elapsed += (sender, e) =>
                {
                    Move();
                };
                _timer.Start();
                _started = true;
            }
        }

        public void Stop()
        {
            if (_started)
            {
                _timer.Stop();
                _started = false;
            }
        }

        /// <summary>
        /// 添加延迟任务
        /// </summary>
        /// <param name="taskAction">处理事件</param>
        /// <param name="seconds">延迟多少秒执行</param>
        public void AddTask(Action taskAction, int seconds)
        {

            var taskContext = new TaskContent()
            {
                CycleNum = seconds / _slotNum,//周期
                TaskAction = taskAction
            };

            _queues.ElementAt((seconds % _slotNum) + _currentIndex).Add(taskContext);

        }

        private void Move()
        {
            int index = _currentIndex;

            //如果当前下标等于插槽数 再从0开始
            if (_currentIndex == (_slotNum - 1))
                _currentIndex = 0;
            else
                _currentIndex++;

            var tasks = _queues[index];

            if (tasks.Count > 0)
            {
                var executeTasks = tasks.Where(q => q.CycleNum <= 0).ToList();

                var awaitTasks = tasks.Where(q => q.CycleNum > 0).Select(t => new TaskContent
                {
                    CycleNum = t.CycleNum - 1,
                    TaskAction = t.TaskAction
                }).ToList();

                _queues[index] = awaitTasks;

                if (executeTasks.Count > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        foreach (var item in executeTasks)
                        {
                            try
                            {
                                item.TaskAction();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    });
                }

            }
        }


        class TaskContent
        {
            /// <summary>
            /// 执行的方法
            /// </summary>
            public Action TaskAction;
            /// <summary>
            /// 周期
            /// </summary>
            public int CycleNum = 0;

        }


    }
}
