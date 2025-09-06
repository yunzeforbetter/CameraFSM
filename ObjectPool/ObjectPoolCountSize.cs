using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Common.Editor
{
#if UNITY_EDITOR
    public class ObjectPoolCountSize : MonoBehaviour
    {
        static ObjectPoolCountSize _instance = null;
        public static ObjectPoolCountSize instance
        {
            get
            {
                return _instance;
            }
        }


        void Awake()
        {
            _instance = this;
        }
        #region 统计池子的最佳大小

        public enum poolOpType
        {
            New,
            Store,
            Pop,
        }
        /// <summary>
        /// 对象池 new 对象的时间间隔统计
        /// </summary>
        public Dictionary<string, Dictionary<poolOpType, List<DateTime>>> pools = new Dictionary<string, Dictionary<poolOpType, List<DateTime>>>();

        public void CountOperate(string name, poolOpType type)
        {
            if (!pools.ContainsKey(name))
            {
                Dictionary<poolOpType, List<DateTime>> one = new Dictionary<poolOpType, List<DateTime>>();
                one.Add(type, new List<DateTime>());
                pools.Add(name, one);
            }
            else
            {
                if (!pools[name].ContainsKey(type))
                {
                    pools[name].Add(type, new List<DateTime>());
                }
            }
            pools[name][type].Add(DateTime.Now);
        }

        DateTime standerDt = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        public void PrintCountResult()
        {
            foreach (var pool in pools)
            {
                string path = Application.persistentDataPath + "\\" + pool.Key + ".log";
                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();
                }
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    foreach (var one in pool.Value)
                    {
                        string head = one.Key + ", ";
                        foreach (var dt in one.Value)
                        {
                            sw.WriteLine(head + (dt - standerDt).TotalMilliseconds);
                        }
                    }
                    sw.Close();
                }

            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical("box");
            foreach (var info in pools)
            {
                GUILayout.Label(info.Key);
                GUILayout.BeginHorizontal();
                foreach (var c in info.Value)
                {
                    GUILayout.Label(c.Key.ToString());
                    GUILayout.Label(c.Value.Count.ToString());
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("打印池"))
                PrintCountResult();
            GUILayout.EndVertical();
        }
        #endregion
    }
#endif

}
