using System;
using UnityEngine;
public partial class TimeCallBack : MonoBehaviour
{
    public class CValueFromToForFactorChangeCallBack : CCallBackInfo, IDisposable
    {
        public float fFactor = 0f;
        public float fFrom = 0f;
        public float fTo = 0f;
        private float fCur = 0f;

        public DelegateValueFromToForFactorChangeCallBack EventValueFromToForFactorChangeCallBack = null;
        public AnimationCurve animationCurve = null; //new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        override public E_CallBackInfoType CallBackType { get { return E_CallBackInfoType.E_ValueFromToForFactor; } }

        protected override void OnTick(float fRealDelta)
        {
            base.OnTick(fRealDelta);
            float amountPerDelta = Mathf.Abs((fDuration > 0f) ? 1f / fDuration : 1000f);
            fFactor += amountPerDelta * fRealDelta;
            float fTempFactor = Sample(fFactor);
            fCur = fFrom * (1f - fTempFactor) + fTo * fTempFactor;
        }

        public override void CallBack(bool bFinish = false)
        {
            if (EventValueFromToForFactorChangeCallBack != null)
            {
                EventValueFromToForFactorChangeCallBack(iTarget, fCur, Sample(fFactor));
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
            fFrom = 0f;
            fTo = 0f;
            fCur = 0f;
            EventValueFromToForFactorChangeCallBack = null;
            animationCurve = null;
            base.Dispose();
        }
    }
}
