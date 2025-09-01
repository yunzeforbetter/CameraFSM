using UnityEngine;
using System;
public partial class TimeCallBack : MonoBehaviour
{
    public class CNormalCallBackInfo : CCallBackInfo, IDisposable
    {
        public DelegateNormalCallBack EventNormalCallBack = null;

        override public E_CallBackInfoType CallBackType { get { return E_CallBackInfoType.E_NormalCallBack; } }

        public override void CallBack(bool bFinish = false)
        {
            EventNormalCallBack?.Invoke(iTarget);
        }

        public override void Dispose()
        {
            EventNormalCallBack = null;
            base.Dispose();
        }
    }
}
