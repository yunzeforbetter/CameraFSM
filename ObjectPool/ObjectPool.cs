using Common.Editor;
using System;
using System.Collections.Generic;

namespace Common.Game
{
    public class ObjectPool<T> where T : class, IDisposable, new()
    {

        #region 自管理
        static Dictionary<string, ObjectPool<T>> allPools = null;

        /// <summary>
        /// 创建类对象池
        /// </summary>
        /// <param name="poolName">类对象池名字，唯一不重复的，建议使用类名</param>
        /// <param name="capacity">类对象池默认容量。设置一个合理的容量，可以减少扩容次数，减少GC</param>
        /// <param name="autoClear">是否由池自动管理对象的清理</param>
        /// <returns></returns>
        public static ObjectPool<T> CreatePool(string poolName, int capacity = 8, bool autoClear = true,int keepCount = -1)
        {
            if (allPools == null)
            {
                allPools = new Dictionary<string, ObjectPool<T>>();
            }
            if (!allPools.ContainsKey(poolName))
            {
                allPools.Add(poolName, new ObjectPool<T>(poolName, capacity, autoClear, keepCount));
            }
            return allPools[poolName];
        }

        /// <summary>
        /// 清理全部池
        /// </summary>
        public static void ClearAllPool()
        {
            if (allPools != null)
            {
                foreach (var pool in allPools)
                {
                    pool.Value.Clear();
                }
                allPools.Clear();
            }
        }
        #endregion

        private readonly Stack<T> mStack;      //存放对象的池子，用List等动态数组也可以，推荐泛型数组

        /// <summary>
        /// 总容量
        /// </summary>
        public int capacity { get; private set; }

        /// <summary>
        /// 创建了多少个
        /// </summary>
        public int countNew { get; private set; }

        /// <summary>
        /// 有多少入池了
        /// </summary>
        public int countRelease { get; private set; }

        private string poolName = string.Empty;

        bool isAutoClear = true;//是否根据autoClearMin时间自动释放
        int storeDT = -1;
        /// <summary>
        /// 自动释放分钟
        /// 入池和出池操作间隔大于此分钟，则主动释放池引用
        /// </summary>
        readonly int autoClearMin = 60 * 1000;//1分钟一次 5秒太短了
        public int KeepCount { get; private set; }
        public ObjectPool(string name, int capacity = 8, bool autoClear = true,int keepCount = -1)
        {
            KeepCount = keepCount;
            this.capacity = capacity;
            mStack = new Stack<T>(capacity);
            poolName = name;
            isAutoClear = autoClear;
        }
        //从池子中获取对象的方法，思路是若池子的数量为0，则调用创建新对象委托创建一个对象返回
        //否则从池子中拿出一个对象并返回
        public T New()
        {
            if (mStack.Count == 0)
            {
                T t = new T();
                countNew++;
#if UNITY_EDITOR
                if (ObjectPoolCountSize.instance != null)
                {
                    ObjectPoolCountSize.instance.CountOperate(poolName, ObjectPoolCountSize.poolOpType.New);
                }
#endif
                return t;
            }
            else
            {
                T t = mStack.Pop();
#if UNITY_EDITOR
                if (ObjectPoolCountSize.instance != null)
                    ObjectPoolCountSize.instance.CountOperate(poolName, ObjectPoolCountSize.poolOpType.Pop);
#endif
                if (isAutoClear && storeDT != -1)
                {
                    if ((Environment.TickCount - storeDT) > autoClearMin)
                    {
                        storeDT = Environment.TickCount;
                        AutoClear();
                    }
                }
                return t;
            }
        }

        //此方法用于将销毁的对象存入池子
        public void Store(T t)
        {
            if (t != null)
            {
                t.Dispose();
                mStack.Push(t);
                countRelease++;
                storeDT = Environment.TickCount;
#if UNITY_EDITOR
                if (ObjectPoolCountSize.instance != null)
                    ObjectPoolCountSize.instance.CountOperate(poolName, ObjectPoolCountSize.poolOpType.Store);
#endif
            }   
        }

        public void AutoClear()
        {
            if(KeepCount > 0)
            {
                if(mStack.Count > KeepCount)
                {
                    countRelease -= (KeepCount - mStack.Count);
                }

                while (mStack.Count > KeepCount)
                {
                    mStack.Pop();
                }
            }
            else
            {
                Clear();
            }
        }

        //清空池
        public void Clear()
        {
            countRelease = 0;
            countNew = 0;
            mStack.Clear();
        }

        /// <summary>
        /// 子类对象池，池里面存的是当前类(T)的子类对象
        /// </summary>
        /// <typeparam name="F">子类</typeparam>
        /// <returns>子类对象</returns>
        public F New<F>() where F : class, T, IDisposable, new()
        {
            if (mStack.Count == 0)
            {
                F f = new F();
                countNew++;
#if UNITY_EDITOR
                if (ObjectPoolCountSize.instance != null)
                {
                    ObjectPoolCountSize.instance.CountOperate(poolName, ObjectPoolCountSize.poolOpType.New);
                }
#endif
                return f;
            }
            else
            {
                F f = mStack.Pop() as F;
#if UNITY_EDITOR
                if (ObjectPoolCountSize.instance != null)
                    ObjectPoolCountSize.instance.CountOperate(poolName, ObjectPoolCountSize.poolOpType.Pop);
#endif
                if (isAutoClear && storeDT != -1)
                {
                    if ((Environment.TickCount - storeDT) > autoClearMin)
                    {
                        storeDT = Environment.TickCount;
                        Clear();
                    }
                }
                return f;
            }
        }
    }
}
