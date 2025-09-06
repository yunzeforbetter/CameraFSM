using System;
using UnityEngine;

public partial class TimeCallBack : MonoBehaviour
{
    public class CCallBackInfo : IDisposable
    {
        public System.Object iTarget = 0;
        private float m_fDuration = 0;

        public float fDuration
        {
            get { return m_fDuration; }
            set
            {
                m_fDuration = value;
                nEndTime = (int) (m_fDuration * 1000) + System.Environment.TickCount;
            }
        }

        int nEndTime = 0;

        public float fSurplus
        {
            get
            {
                int nSurplus = nEndTime - System.Environment.TickCount;
                return nSurplus / 1000f;
            }
        }

        /// <summary>
        /// stop之后置为true，等待update里取移除
        /// </summary>
        public bool WaitForRemove { get; protected set; } = false;

        public E_Style eStyle = E_Style.E_Style_Once;

        public virtual E_CallBackInfoType CallBackType
        {
            get { return E_CallBackInfoType.E_NormalCallBack; }
        }

        protected virtual void OnTick(float fRealDelta)
        {
        }

        public virtual void CallBack(bool bFinish = false)
        {
            
        }

        public bool TickCheckFinish(float fRealDelta, ref bool bNeedCallBack)
        {
            OnTick(fRealDelta);
            if (CheckFinish(fRealDelta))
            {
                if (eStyle == E_Style.E_Style_Repeat)
                {
                    bNeedCallBack = true;
                    fDuration = m_fDuration;
                    return false;
                }

                return true;
            }
            else if (eStyle == E_Style.E_Style_EveryFrame)
            {
                bNeedCallBack = true;
            }
            else
            {
                bNeedCallBack = false;
            }

            return false;
        }

        protected virtual bool CheckFinish(float fRealDelta)
        {
            return System.Environment.TickCount >= nEndTime;
        }

        public virtual void Finish()
        {
            CallBack(true);
        }

        public void Stop()
        {
            WaitForRemove = true;
        }

        public virtual void Dispose()
        {
            WaitForRemove = false;
        }
    }
}