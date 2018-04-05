using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Hong.Algorithms
{
    public class Queue<T>
    {
        /// <summary>
        /// 数据信息,为避免内存复制使用此类
        /// </summary>
        class QueueDataInfo
        {
            public bool IsEmpty = false;

            public T Data = default(T);
        }

        /// <summary>
        /// 队列头
        /// </summary>
        int head = -1;

        /// <summary>
        /// 队列尾
        /// </summary>
        int tail = -1;

        /// <summary>
        /// 数据大小
        /// </summary>
        readonly int dataSize = 4000;

        /// <summary>
        /// 数据
        /// </summary>
        QueueDataInfo[] datas;

        public Queue(int size)
        {
            dataSize = size;
            datas = new QueueDataInfo[size];
        }

        /// <summary>
        /// 加入队列
        /// </summary>
        /// <exception cref="Exception">如果队列提取根据不上将等待,可能超时</exception>
        /// <param name="item"></param>
        public void Enque(T item)
        {
            var current = head;
            var myHead = current;
            var cHead = -2;
            QueueDataInfo cDataInfo = null;

            //得到排队编号,循环直到取到编号
            while (cHead != current)
            {
                cHead = myHead = current;
                if (++myHead == dataSize)
                {
                    myHead = 0;
                }

                //防止生产太快,消息太慢的情况
                if (tail == myHead)
                {
                    throw new Exception("出队速度太慢,无法跟上入队速度");
                }

                current = Interlocked.CompareExchange(ref head, myHead, cHead);
            }

            cDataInfo = datas[myHead];
            if (cDataInfo == null)
            {
                datas[myHead] = new QueueDataInfo()
                {
                    IsEmpty = false,
                    Data = item
                };

                return;
            }

            cDataInfo.Data = item;
        }

        /// <summary>
        /// 尝试加入队列
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryEnque(T item)
        {
            try
            {
                Enque(item);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <param name="defaultValue">队列为空时默认值</param>
        /// <returns></returns>
        public T Deque(T defaultValue = default(T))
        {
            var current = tail; //最新位置
            var next = current;
            var cTail = current;
            var cHead = head;
            QueueDataInfo cDataInfo = null;

            //取值直到没有数据
            while (current < cHead)
            {
                //计算下一个位置
                if (++next == dataSize)
                {
                    next = 0;
                }

                //尝试确立我的位置
                current = Interlocked.CompareExchange(ref tail, next, cTail);
                if (current == cTail)
                {
                    cDataInfo = datas[next];
                    if (cDataInfo != null && !cDataInfo.IsEmpty)
                    {
                        //为防止加入队列时途失败或线程中止情况,只有取得真正的值,退出
                        cDataInfo.IsEmpty = true;

                        return cDataInfo.Data;
                    }
                }

                cTail = next = current;
            }

            return defaultValue;
        }
    }
}
