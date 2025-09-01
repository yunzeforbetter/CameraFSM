using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// 相机震屏节点
    /// </summary>
    public class CameraShakeNode : CameraAction
    {
        [Header("震屏持续时间")]
        public float Duration;
        [Header("震屏频率"),Range(0,1)]
        public float ShakeSensitive;

        public override bool DoAction()
        {
            CMCameraManager.Instance.PlayShake(Duration, ShakeSensitive);
            TimeCallBack.StopAllSelf(this);
            TimeCallBack.BeginForNormalCallBack(this, Duration, o =>
            {
                base.DoAction();
            });
            return true;
        }

        public override void Release()
        {
            CMCameraManager.Instance.StopShake();
            TimeCallBack.StopAllSelf(this);
        }
    }
}
