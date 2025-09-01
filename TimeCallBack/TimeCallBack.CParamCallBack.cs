using UnityEngine;
using System;
public partial class TimeCallBack : MonoBehaviour
{
    public class CParamCallBack : CCallBackInfo,IDisposable
    {
        public System.Object objParam = null;
        public DelegateParamCallBack EventParamCallBack = null;

        override public E_CallBackInfoType CallBackType { get { return E_CallBackInfoType.E_ParamCallBack; } }

        public override void CallBack(bool bFinish = false)
        {
            if (EventParamCallBack != null)
            {
                EventParamCallBack(iTarget, objParam);
                if (bFinish)
                {
                    EventParamCallBack = null;
                }
            }
        }
        public override void Dispose()
        {
            objParam = null;
            EventParamCallBack = null;
            base.Dispose();
        }
    }
}
