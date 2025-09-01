using System;
using UnityEngine;
public partial class TimeCallBack : MonoBehaviour
{
    public class CTotalConsumeTimeCallBack : CCallBackInfo, IDisposable
    {
        public System.Object objParam = null;
        public DelegateTotalConsumeTimeCallBack EventTotalConsumeTimeCallBack = null;
        public float fTotalConsumeTime = 0;

        override public E_CallBackInfoType CallBackType { get { return E_CallBackInfoType.E_TotalConsumeTimeCallBack; } }

        protected override void OnTick(float fRealDelta)
        {
            base.OnTick(fRealDelta);

            fTotalConsumeTime += fRealDelta;
        }

        public override void CallBack(bool bFinish = false)
        {
            if (EventTotalConsumeTimeCallBack != null)
            {
                EventTotalConsumeTimeCallBack(this, iTarget, fTotalConsumeTime);
            }
        }

        protected override bool CheckFinish(float fRealDelta)
        {
            if (fTotalConsumeTime > 10000)
            {
                return true;
            }
            return false;
        }

        public override void Dispose()
        {
            objParam = null;
            EventTotalConsumeTimeCallBack = null;
            fTotalConsumeTime = 0;
            base.Dispose();
        }
    }
}
