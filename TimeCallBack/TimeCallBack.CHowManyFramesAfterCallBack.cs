using UnityEngine;
using System;
public partial class TimeCallBack : MonoBehaviour
{
    public class CHowManyFramesAfterCallBack : CCallBackInfo,IDisposable
    {
        public int nHowManyFramesAfter = 0;
        public DelegateHowManyFramesAfterCallBack EventHowManyFramesAfterCallBack = null;

        override public E_CallBackInfoType CallBackType { get { return E_CallBackInfoType.E_HowManyFramesAfterCallBack; } }

        protected override void OnTick(float fRealDelta)
        {
            base.OnTick(fRealDelta);

            nHowManyFramesAfter--;
        }
        
        protected override bool CheckFinish(float fRealDelta)
        {
            return nHowManyFramesAfter <= 0;
        }

        public override void CallBack(bool bFinish = false)
        {
            if (EventHowManyFramesAfterCallBack != null)
            {
                EventHowManyFramesAfterCallBack(iTarget);
            }
        }

        public override void Dispose()
        {
            nHowManyFramesAfter = 0;
            EventHowManyFramesAfterCallBack = null;
            base.Dispose();
        }
    }
}
