
using UnityEngine;

namespace CMCameraFramework
{
    [System.Serializable]
    public class ZoomFromToEvent : CameraEvent
    {
        public float ZoomCurDis;
        public float ZoomTargetDis;
        public bool IsHoldZoom;
        public float ZoomTweenTime;
        public AnimationCurve TweenCurve;
    }
}