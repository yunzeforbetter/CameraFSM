using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Invector
{
    public static class vExtensions
    {
        public static T[] Append<T>(this T[] arrayInitial, T[] arrayToAppend)
        {
            if (arrayToAppend == null)
            {
                throw new ArgumentNullException("The appended object cannot be null");
            }

            if ((arrayInitial is string) || (arrayToAppend is string))
            {
                throw new ArgumentException("The argument must be an enumerable");
            }

            T[] ret = new T[arrayInitial.Length + arrayToAppend.Length];
            arrayInitial.CopyTo(ret, 0);
            arrayToAppend.CopyTo(ret, arrayInitial.Length);

            return ret;
        }

        /// <summary>
        /// Normalized the angle. between -180 and 180 degrees
        /// </summary>
        /// <param Name="eulerAngle">Euler angle.</param>
        public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
        {
            var delta = eulerAngle;

            if (delta.x > 180) delta.x -= 360;
            else if (delta.x < -180) delta.x += 360;

            if (delta.y > 180) delta.y -= 360;
            else if (delta.y < -180) delta.y += 360;

            if (delta.z > 180) delta.z -= 360;
            else if (delta.z < -180) delta.z += 360;

            return new Vector3(delta.x, delta.y, delta.z); //round values to angle;
        }

        public static Vector3 Difference(this Vector3 vector, Vector3 otherVector)
        {
            return otherVector - vector;
        }

        public static void SetActiveChildren(this GameObject gameObjet, bool value)
        {
            foreach (Transform child in gameObjet.transform)
                child.gameObject.SetActive(value);
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360)
                    angle += 360;
                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }

        public static ClipPlanePoints NearClipPlanePoints(this Camera camera, Vector3 pos, float clipPlaneMargin,
            float clipOffset = 0f)
        {
            var clipPlanePoints = new ClipPlanePoints();
            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;
            height *= 1 + clipPlaneMargin;
            width *= 1 + clipPlaneMargin;
            clipPlanePoints.width = width * 2;
            clipPlanePoints.heigth = height * 2;

            height += clipOffset;
            width += clipOffset;

            var right = transform.right;
            var up = transform.up;
            var forward = transform.forward;

            clipPlanePoints.LowerRight = pos + right * width;
            clipPlanePoints.LowerRight -= up * height;
            clipPlanePoints.LowerRight += forward * distance;

            clipPlanePoints.LowerLeft = pos - right * width;
            clipPlanePoints.LowerLeft -= up * height;
            clipPlanePoints.LowerLeft += forward * distance;

            clipPlanePoints.UpperRight = pos + right * width;
            clipPlanePoints.UpperRight += up * height;
            clipPlanePoints.UpperRight += forward * distance;

            clipPlanePoints.UpperLeft = pos - right * width;
            clipPlanePoints.UpperLeft += up * height;
            clipPlanePoints.UpperLeft += forward * distance;

            return clipPlanePoints;
        }

        public static HitBarPoints GetBoundPoint(this BoxCollider boxCollider, Transform torso, LayerMask mask)
        {
            HitBarPoints bp = new HitBarPoints();
            var boxPoint = boxCollider.GetBoxPoint();
            Ray toTop = new Ray(boxPoint.top, boxPoint.top - torso.position);
            Ray toCenter = new Ray(torso.position, boxPoint.center - torso.position);
            Ray toBottom = new Ray(torso.position, boxPoint.bottom - torso.position);
#if UNITY_EDITOR
            Debug.DrawRay(toTop.origin, toTop.direction, Color.red, 2);
            Debug.DrawRay(toCenter.origin, toCenter.direction, Color.green, 2);
            Debug.DrawRay(toBottom.origin, toBottom.direction, Color.blue, 2);
#endif
            RaycastHit hit;
            var dist = Vector3.Distance(torso.position, boxPoint.top);
            if (Physics.Raycast(toTop, out hit, dist, mask))
            {
                bp |= HitBarPoints.Top;
                Debug.Log(hit.transform.name);
            }

            dist = Vector3.Distance(torso.position, boxPoint.center);
            if (Physics.Raycast(toCenter, out hit, dist, mask))
            {
                bp |= HitBarPoints.Center;
                Debug.Log(hit.transform.name);
            }

            dist = Vector3.Distance(torso.position, boxPoint.bottom);
            if (Physics.Raycast(toBottom, out hit, dist, mask))
            {
                bp |= HitBarPoints.Bottom;
                Debug.Log(hit.transform.name);
            }

            return bp;
        }

        public static BoxPoint GetBoxPoint(this BoxCollider boxCollider)
        {
            BoxPoint bp = new BoxPoint();
            bp.center = boxCollider.transform.TransformPoint(boxCollider.center);
            var height = boxCollider.transform.lossyScale.y * boxCollider.size.y;
            var ray = new Ray(bp.center, boxCollider.transform.up);

            bp.top = ray.GetPoint((height * 0.5f));
            bp.bottom = ray.GetPoint(-(height * 0.5f));

            return bp;
        }

        public static Vector3 BoxSize(this BoxCollider boxCollider)
        {
            var length = boxCollider.transform.lossyScale.x * boxCollider.size.x;
            var width = boxCollider.transform.lossyScale.z * boxCollider.size.z;
            var height = boxCollider.transform.lossyScale.y * boxCollider.size.y;
            return new Vector3(length, height, width);
        }

        public static bool Contains(this Enum keys, Enum flag)
        {
            if (keys.GetType() != flag.GetType())
                throw new ArgumentException("Type Mismatch");
            return (Convert.ToUInt64(keys) & Convert.ToUInt64(flag)) != 0;
        }

        public static T ConvertToType<T>(this string input)
        {
            var isConverted = false;
            var type = typeof(T);
            if (type == typeof(string))
            {
                return (T) (object) input;
            }
            else if (type == typeof(float))
            {
                float f;
                isConverted = float.TryParse(input, out f);
                if (isConverted)
                {
                    return (T) (object) f;
                }
            }
            else if (type == typeof(int))
            {
                int i;
                isConverted = int.TryParse(input, out i);
                if (isConverted)
                {
                    return (T) (object) i;
                }
            }
            else if (type == typeof(bool))
            {
                bool b;
                isConverted = bool.TryParse(input, out b);
                if (isConverted)
                {
                    return (T) (object) b;
                }
            }

            return default(T);
        }
        
        public static bool IsValidAsType(this string input, Type type)
        {
            var isConverted = false;
            if (type == typeof(string))
            {
                isConverted = true;
            }
            else if (type == typeof(float))
            {
                float f;
                isConverted = float.TryParse(input, out f);
            }
            else if (type == typeof(int))
            {
                int i;
                isConverted = int.TryParse(input, out i);
            }
            else if (type == typeof(bool))
            {
                bool b;
                isConverted = bool.TryParse(input, out b);
            }

            return isConverted;
        }

    }

    public struct BoxPoint
    {
        public Vector3 top;
        public Vector3 center;
        public Vector3 bottom;
    }

    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;

        public Vector3 LowerRight;

        //public Vector3 center;
        //public Vector3 Right;
        //public Vector3 Left;
        //public Vector3 Top;
        //public Vector3 Lower;
        public float width;
        public float heigth;
    }

    [Flags]
    public enum HitBarPoints
    {
        None = 0,
        Top = 1,
        Center = 2,
        Bottom = 4
    }
}