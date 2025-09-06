using System;
using System.Collections.Generic;
using Common.Game;
using UnityEngine;

public partial class TimeCallBack : MonoBehaviour
{
    private Dictionary<System.Type, ObjectPool<CCallBackInfo>> TimeCallPools =
        new Dictionary<Type, ObjectPool<CCallBackInfo>>();

    private ObjectPool<CCallBackInfo> GetPool(System.Type t)
    {
        ObjectPool<CCallBackInfo> r = null;
        if (!TimeCallPools.TryGetValue(t, out r))
        {
            r = new ObjectPool<CCallBackInfo>(t.Name, 20, false);
            TimeCallPools.Add(t, r);
        }

        return r;
    }

    /// <summary>
    /// 获取一个CallbackInfo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T NewInfo<T>() where T : CCallBackInfo, IDisposable, new()
    {
        var p = GetPool(typeof(T));
        return p.New<T>();
    }

    private void StoreInfo<T>(T t) where T : CCallBackInfo, IDisposable, new()
    {
        var p = GetPool(t.GetType());
        p.Store(t);
    }
}