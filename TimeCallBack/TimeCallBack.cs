using System.Collections.Generic;
using UnityEngine;

public partial class TimeCallBack : MonoBehaviour
{
    public delegate void DelegateNormalCallBack(System.Object iTarget);
    public delegate void DelegateNormalCallBackNoObject(object iTarget);
    public delegate void DelegateFactorCallBack(System.Object iTarget, float fFactor);
    public delegate void DelegateValueFromToForFactorChangeCallBack(System.Object iTarget, float fNowValue, float fNowFactor);
    public delegate void DelegateValueFromToForFactorNoCalculateCallBack(System.Object iTarget, object objFrom, object objTo, float fNowFactor);
    public delegate void DelegateParamCallBack(System.Object iTarget, System.Object objParam);
    public delegate void DelegateHowManyFramesAfterCallBack(System.Object iTarget);
    public delegate void DelegateTotalConsumeTimeCallBack(TimeCallBack.CCallBackInfo aTime, System.Object iTarget, float fTotalConsumeTime);

    public enum E_Style
    {
        E_Style_Once,
        E_Style_Repeat,
        E_Style_EveryFrame,
    }

    public enum E_CallBackInfoType
    {
        E_NormalCallBack = 1,
        E_FactorCallBack,
        E_ValueFromToForFactor,
        E_ValueFromToForFactotNoCalculate,
        E_ParamCallBack,
        E_HowManyFramesAfterCallBack,
        E_TotalConsumeTimeCallBack,
    }

    public class Timer
    {

    }

    static private GameObject m_aActionCallBackObj = null;
    List<CCallBackInfo> m_ltCallBack = new List<CCallBackInfo>();
    List<CCallBackInfo> m_ltFinish = new List<CCallBackInfo>();	// 原本的局部变量，定义为成员变量节省GC yao 20140926
    List<CCallBackInfo> m_ltNeedCallBack = new List<CCallBackInfo>();
    
    float m_fRealTime = 0f;
    float m_fRealDelta = 0f;

    static TimeCallBack m_aActionComponent = null;
    public static TimeCallBack ActionComponent
    {
        get
        {
            if (m_aActionComponent == null)
            {
                m_aActionComponent = ActionCallBackObj.AddComponent<TimeCallBack>();
            }
            return m_aActionComponent;
        }
    }

    static public GameObject ActionCallBackObj
    {
        get
        {
            if (null == m_aActionCallBackObj)
            {
                m_aActionCallBackObj = new GameObject("ActionCallBackObj");
                GameObject.DontDestroyOnLoad(m_aActionCallBackObj);
            }

            return m_aActionCallBackObj;
        }
    }

    public float fDt = 0;
    void Update()
    {
        {
            float rt = Time.realtimeSinceStartup;

            if (null == m_ltCallBack || m_ltCallBack.Count <= 0)
            {
                m_fRealTime = rt;
                this.enabled = false;
                return;
            }

            m_fRealDelta = Mathf.Clamp(rt - m_fRealTime, 0, float.MaxValue);
            m_fRealTime = rt;
            fDt = m_fRealDelta;
            m_ltNeedCallBack.Clear();
            m_ltFinish.Clear();
            int nCount = m_ltCallBack.Count;
            for (int i = 0; i < nCount; ++i)
            {
                CCallBackInfo aInfo = m_ltCallBack[i];
                if (null == aInfo)
                {
                    m_ltFinish.Add(aInfo);
                    continue;
                }

                if (aInfo.WaitForRemove)
                {
                    m_ltFinish.Add(aInfo);
                    continue;
                }

                bool bNeedCallBack = false;

                if (aInfo.TickCheckFinish(m_fRealDelta, ref bNeedCallBack))
                {
                    m_ltFinish.Add(aInfo);
                }
                if (bNeedCallBack)
                {
                    m_ltNeedCallBack.Add(aInfo);
                }
            }

            for (int i = 0; i < m_ltNeedCallBack.Count; ++i)
            {
                CCallBackInfo aInfo = m_ltNeedCallBack[i];
                if (aInfo != null)
                {
                    aInfo.CallBack();
                }
            }

            for (int i = 0; i < m_ltFinish.Count; ++i)
            {
                CCallBackInfo aInfo = m_ltFinish[i];
                if (aInfo != null && aInfo.WaitForRemove == false)
                {
                    aInfo.Finish();
                }

                m_ltCallBack.Remove(aInfo);
                ActionComponent.StoreInfo(aInfo);
            }
        }
    }

    public static void BeginForNormalCallBack(System.Object iTarget, float fDuration, DelegateNormalCallBack NormalCallBack = null, E_Style eStyle = E_Style.E_Style_Once)
    { 
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }
        CNormalCallBackInfo aInfo = ActionComponent.NewInfo<CNormalCallBackInfo>();
        aInfo.iTarget = iTarget;
        aInfo.fDuration = fDuration;
        //aInfo.fSurplus = fDuration;
        aInfo.EventNormalCallBack = NormalCallBack;
        aInfo.eStyle = eStyle;
        ActionComponent.m_ltCallBack.Add(aInfo);
    }

    static public void BeginForFactorCallBack(System.Object iTarget, float fDuration, DelegateFactorCallBack FactorCallBack, AnimationCurve animationCurve = null)
    {
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }

        CFactorCallBack aInfo = ActionComponent.NewInfo<CFactorCallBack>();
        aInfo.iTarget = iTarget;
        aInfo.fDuration = fDuration;
        //aInfo.fSurplus = fDuration;
        aInfo.fFactor = 0f;
        aInfo.EventFactorCallBack = FactorCallBack;
        aInfo.animationCurve = animationCurve;
        aInfo.eStyle = E_Style.E_Style_EveryFrame;
        ActionComponent.m_ltCallBack.Add(aInfo);
    }

    static public void BeginValueFromToForFactorChangeCallBack(System.Object iTarget, float fDuration, float fFromValue, float fToValue, DelegateValueFromToForFactorChangeCallBack ValueFromToForFactorChangeCallBack, AnimationCurve animationCurve = null)
    {
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }
        CValueFromToForFactorChangeCallBack aInfo = ActionComponent.NewInfo<CValueFromToForFactorChangeCallBack>();
        aInfo.iTarget = iTarget;
        aInfo.fDuration = fDuration;
        //aInfo.fSurplus = fDuration;
        aInfo.fFactor = 0f;
        aInfo.fFrom = fFromValue;
        aInfo.fTo = fToValue;
        aInfo.EventValueFromToForFactorChangeCallBack = ValueFromToForFactorChangeCallBack;
        aInfo.animationCurve = animationCurve;
        aInfo.eStyle = E_Style.E_Style_EveryFrame;
        ActionComponent.m_ltCallBack.Add(aInfo);
    }

    //不直接计算最终值 抛回去使用者自己计算
    static public void BeginValueFromToForFactorNoCalculateCallBack(System.Object iTarget, float fDuration, object objFromValue, object objToValue, DelegateValueFromToForFactorNoCalculateCallBack callback, AnimationCurve animationCurve = null)
    {
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }
        CValueFromToForFactorNoCalculateCallBack aInfo = ActionComponent.NewInfo<CValueFromToForFactorNoCalculateCallBack>();
        aInfo.iTarget = iTarget;
        aInfo.fDuration = fDuration;
        //aInfo.fSurplus = fDuration;
        aInfo.fFactor = 0f;
        aInfo.objFrom = objFromValue;
        aInfo.objTo = objToValue;
        aInfo.EventValueFromToForFactorNoCalculateCallBack = callback;
        aInfo.animationCurve = animationCurve;
        aInfo.eStyle = E_Style.E_Style_EveryFrame;
        ActionComponent.m_ltCallBack.Add(aInfo);
    }

    static public void BeginForParamCallBack(System.Object iTarget, float fDuration, System.Object objParam, DelegateParamCallBack ParamCallBack = null, E_Style eStyle = E_Style.E_Style_Once)
    {
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }
        CParamCallBack aInfo = ActionComponent.NewInfo<CParamCallBack>();
        aInfo.iTarget = iTarget;
        aInfo.fDuration = fDuration;
        //aInfo.fSurplus = fDuration;

        aInfo.objParam = objParam;
        aInfo.EventParamCallBack = ParamCallBack;
        aInfo.eStyle = eStyle;
        ActionComponent.m_ltCallBack.Add(aInfo);
    }

    static public void BeginForHowManyFramesAfterCallBack(System.Object iTarget, int nHowManyFramesAfter, DelegateHowManyFramesAfterCallBack HowManyFramesAfterCallBack = null)
    {
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }
        CHowManyFramesAfterCallBack aInfo = ActionComponent.NewInfo<CHowManyFramesAfterCallBack>();
        aInfo.iTarget = iTarget;
        aInfo.fDuration = 0f;
        //aInfo.fSurplus = 0f;
        aInfo.nHowManyFramesAfter = nHowManyFramesAfter;
        aInfo.EventHowManyFramesAfterCallBack = HowManyFramesAfterCallBack;
        aInfo.eStyle = E_Style.E_Style_Once;
        ActionComponent.m_ltCallBack.Add(aInfo);
    }

    // 秒表计时器，设置多少秒后结束，回调消耗总时间 [2/28/2016 任修]
    // 在结束之前可以不断修改结束时间 [2/28/2016 任修]
    // 例如：第一次设置5秒后结束并回调， 当进行到第二秒时 再次调用这个接口设置为6秒后结束并回调 那么定时器会在第8秒后回调 消耗用时8秒钟 [2/28/2016 任修]
    // 设计目的：浮空过程中会被连续再浮空，需要计算整个浮空上升中消耗了多少时间，以后有类似需求可以使用该接口计算总耗时，不需要再自己的类里加成员计算 [2/28/2016 任修]
    static public void BeginOrUpdateTargetForTotalConsumeTimeCallBack(System.Object iTarget, float fNewSurplusTime, DelegateTotalConsumeTimeCallBack TotalConsumeTimeCallBack = null)
    {
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }

        CTotalConsumeTimeCallBack aInfo = null;
        List<CCallBackInfo> lt = ActionComponent.m_ltCallBack;
        int nCount = lt.Count;
        for (int i = 0; i < nCount; ++i)
        {
            if (lt[i] != null && lt[i].CallBackType == E_CallBackInfoType.E_TotalConsumeTimeCallBack && lt[i].iTarget == iTarget)
            {
                aInfo = (CTotalConsumeTimeCallBack)lt[i];
                //aInfo.fSurplus = fNewSurplusTime;
                aInfo.fDuration = fNewSurplusTime;
                break;
            }
        }

        if (aInfo == null)
        {
            aInfo = ActionComponent.NewInfo<CTotalConsumeTimeCallBack>();
            aInfo.iTarget = iTarget;
            aInfo.fDuration = fNewSurplusTime;
            //aInfo.fSurplus = fNewSurplusTime;
            aInfo.fTotalConsumeTime = 0;
            aInfo.EventTotalConsumeTimeCallBack = TotalConsumeTimeCallBack;
            aInfo.eStyle = E_Style.E_Style_Once;
            ActionComponent.m_ltCallBack.Add(aInfo);
        }
    }

    static public void BeginForTotalConsumeTimeCallBack(System.Object iTarget, DelegateTotalConsumeTimeCallBack TotalConsumeTimeCallBack = null, E_Style eStyle = E_Style.E_Style_EveryFrame)
    {
        if (!ActionComponent.enabled)
        {
            ActionComponent.m_fRealTime = Time.realtimeSinceStartup;
            ActionComponent.enabled = true;
        }

        CTotalConsumeTimeCallBack aInfo = ActionComponent.NewInfo<CTotalConsumeTimeCallBack>();
        aInfo.iTarget = iTarget;
        aInfo.fTotalConsumeTime = 0;
        aInfo.EventTotalConsumeTimeCallBack = TotalConsumeTimeCallBack;
        aInfo.eStyle = eStyle;
        ActionComponent.m_ltCallBack.Add(aInfo);
    }

    static public void Release()
    {
        if (m_aActionCallBackObj == null)
        {
            return;
        }

        GameObject.Destroy(m_aActionCallBackObj);
    }

    static public void StopAllSelf(System.Object iTarget)
    {
        if (m_aActionCallBackObj == null)
        {
            return;
        }

        if (ActionComponent == null)
        {
            return;
        }

        for (int i = 0; i <ActionComponent.m_ltCallBack.Count; i++)
        {
            CCallBackInfo info = ActionComponent.m_ltCallBack[i];
            if (null == info)
            {
                continue;
            }
            if (System.Object.Equals(info.iTarget, iTarget))
            {
                info.Stop();
            }
        }
    }

    /// <summary>
    /// 慎用有唯一target 的才可以返回正确的callbakinfo
    /// </summary>
    /// <param name="iTarget"></param>
    /// <returns></returns>
    public static CCallBackInfo GetCallBackInfoByTarget(object iTarget)
    {
        foreach (var item in ActionComponent.m_ltCallBack)
        {
            if (item != null && item.iTarget == iTarget)
            {
                return item;
            }
        }
        return null;
    }
}
