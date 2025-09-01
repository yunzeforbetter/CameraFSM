using System;
using UnityEngine;
public partial class TimeCallBack : MonoBehaviour
{
    public class CValueFromToForFactorNoCalculateCallBack : CCallBackInfo, IDisposable
    {
        public float fFactor = 0f;
        public object objFrom = 0f;
        public object objTo = 0f;
        public AnimationCurve animationCurve = null;
        public DelegateValueFromToForFactorNoCalculateCallBack EventValueFromToForFactorNoCalculateCallBack = null;

        override public E_CallBackInfoType CallBackType { get { return E_CallBackInfoType.E_ValueFromToForFactotNoCalculate; } }

        protected override void OnTick(float fRealDelta)
        {
            base.OnTick(fRealDelta);
            float amountPerDelta = Mathf.Abs((fDuration > 0f) ? 1f / fDuration : 1000f);
            fFactor += amountPerDelta * fRealDelta;
        }

        public override void CallBack(bool bFinish = false)
        {
            if (EventValueFromToForFactorNoCalculateCallBack != null)
            {
                EventValueFromToForFactorNoCalculateCallBack(iTarget, objFrom, objTo, Sample(fFactor));
            }
        }

        public override void Finish()
        {
            fFactor = 1;
            base.Finish();
        }

        public float Sample(float factor)
        {
            // Calculate the sampling value
            float val = Mathf.Clamp01(factor);
            // Call the virtual update
            return (animationCurve != null) ? animationCurve.Evaluate(val) : val;
        }

        public override void Dispose()
        {
            fFactor = 0f;
            objFrom = null;
            objTo = null;
            animationCurve = null;
            EventValueFromToForFactorNoCalculateCallBack = null;
            base.Dispose();
        }
    }
}
