using UnityEngine;
using System;
public partial class TimeCallBack : MonoBehaviour
{
    public class CFactorCallBack : CCallBackInfo,IDisposable
    {
        public float fFactor = 0f;
        public DelegateFactorCallBack EventFactorCallBack = null;
        public AnimationCurve animationCurve = null; //new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        override public E_CallBackInfoType CallBackType { get { return E_CallBackInfoType.E_FactorCallBack; } }

        protected override void OnTick(float fRealDelta)
        {
            float amountPerDelta = Mathf.Abs((fDuration > 0f) ? 1f / fDuration : 1000f);
            fFactor += amountPerDelta * fRealDelta;
        }

        public override void CallBack(bool bFinish = false)
        {
            if (EventFactorCallBack != null)
            {
                EventFactorCallBack(iTarget, Sample(fFactor));
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
            fFactor = 0;
            EventFactorCallBack = null;
            animationCurve = null;
            base.Dispose();
        }
    }
}
