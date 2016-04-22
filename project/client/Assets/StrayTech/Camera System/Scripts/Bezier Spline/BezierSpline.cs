using UnityEngine;
using System;

namespace StrayTech
{
    public class BezierSpline : MonoBehaviour
    {
        #region inner types
            public enum BezierControlPointMode
            {
                Free,
                Aligned,
                Mirrored
            }
        #endregion inner types

        #region inspector members
            /// <summary>
            /// List of control points.
            /// </summary>
            [SerializeField]
            private Vector3[] _points;

            /// <summary>
            /// Control point handle mode.
            /// </summary>
            [SerializeField]
            private BezierControlPointMode[] _modes;

            /// <summary>
            /// Should the end point of the last curve join with the start point of the first curve?
            /// </summary>
            [SerializeField]
            private bool _loop;

            /// <summary>
            /// Defines how accurately numeric calculations will be done.
            /// </summary>
            [SerializeField]
            private int _interpolationAccuracy = 5;
        #endregion inspector members

        #region members
            private float _length = 0.0f;
        #endregion members

        #region properties
            public bool Loop
            {
                get { return this._loop; }
                set
                {
                    this._loop = value;
                    if (value == true)
                    {
                        this._modes[this._modes.Length - 1] = this._modes[0];
                        SetControlPoint(0, this._points[0]);
                    }
                }
            }

            public float Length { get { return this._length; } }

            public int CurveCount { get { return (this._points.Length - 1) / 3; } }

            public int ControlPointCount { get { return this._points.Length; } }
        #endregion properties

        #region constructors
            private void Awake()
            {
                CalculateSplineLength();
            }
        #endregion constructors

        #region methods
            public Vector3 GetControlPoint(int index)
            {
                return this._points[index];
            }

            public void SetControlPoint(int index, Vector3 point)
            {
                if (index % 3 == 0)
                {
                    Vector3 delta = point - this._points[index];
                    if (this._loop)
                    {
                        if (index == 0)
                        {
                            this._points[1] += delta;
                            this._points[this._points.Length - 2] += delta;
                            this._points[this._points.Length - 1] = point;
                        }
                        else if (index == this._points.Length - 1)
                        {
                            this._points[0] = point;
                            this._points[1] += delta;
                            this._points[index - 1] += delta;
                        }
                        else
                        {
                            this._points[index - 1] += delta;
                            this._points[index + 1] += delta;
                        }
                    }
                    else
                    {
                        if (index > 0)
                        {
                            this._points[index - 1] += delta;
                        }
                        if (index + 1 < this._points.Length)
                        {
                            this._points[index + 1] += delta;
                        }
                    }
                }
                this._points[index] = point;
                EnforceMode(index);
            }

            public BezierControlPointMode GetControlPointMode(int index)
            {
                return this._modes[(index + 1) / 3];
            }

            public void SetControlPointMode(int index, BezierControlPointMode mode)
            {
                int modeIndex = (index + 1) / 3;
                this._modes[modeIndex] = mode;
                if (this._loop)
                {
                    if (modeIndex == 0)
                    {
                        this._modes[_modes.Length - 1] = mode;
                    }
                    else if (modeIndex == this._modes.Length - 1)
                    {
                        this._modes[0] = mode;
                    }
                }
                EnforceMode(index);
            }

            private void EnforceMode(int index)
            {
                int modeIndex = (index + 1) / 3;
                BezierControlPointMode mode = this._modes[modeIndex];
                if (mode == BezierControlPointMode.Free || !this._loop && (modeIndex == 0 || modeIndex == this._modes.Length - 1))
                {
                    return;
                }

                int middleIndex = modeIndex * 3;
                int fixedIndex, enforcedIndex;
                if (index <= middleIndex)
                {
                    fixedIndex = middleIndex - 1;
                    if (fixedIndex < 0)
                    {
                        fixedIndex = this._points.Length - 2;
                    }
                    enforcedIndex = middleIndex + 1;
                    if (enforcedIndex >= this._points.Length)
                    {
                        enforcedIndex = 1;
                    }
                }
                else
                {
                    fixedIndex = middleIndex + 1;
                    if (fixedIndex >= this._points.Length)
                    {
                        fixedIndex = 1;
                    }
                    enforcedIndex = middleIndex - 1;
                    if (enforcedIndex < 0)
                    {
                        enforcedIndex = this._points.Length - 2;
                    }
                }

                Vector3 middle = this._points[middleIndex];
                Vector3 enforcedTangent = middle - this._points[fixedIndex];
                if (mode == BezierControlPointMode.Aligned)
                {
                    enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, this._points[enforcedIndex]);
                }
                this._points[enforcedIndex] = middle + enforcedTangent;
            }

            public Vector3 GetPosition(float t)
            {
                int i;
                if (t >= 1f)
                {
                    t = 1f;
                    i = this._points.Length - 4;
                }
                else
                {
                    t = Mathf.Clamp01(t) * CurveCount;
                    i = (int)t;
                    t -= i;
                    i *= 3;
                }
                return transform.TransformPoint(Bezier.GetPoint(this._points[i], this._points[i + 1], this._points[i + 2], this._points[i + 3], t));
            }

            public Vector3 GetVelocity(float t)
            {
                int i;
                if (t >= 1f)
                {
                    t = 1f;
                    i = this._points.Length - 4;
                }
                else
                {
                    t = Mathf.Clamp01(t) * CurveCount;
                    i = (int)t;
                    t -= i;
                    i *= 3;
                }
                return transform.TransformPoint(Bezier.GetFirstDerivative(this._points[i], this._points[i + 1], this._points[i + 2], this._points[i + 3], t)) - transform.position;
            }

            public Vector3 GetDirection(float t)
            {
                return GetVelocity(t).normalized;
            }

            public void AddCurve()
            {
                Vector3 point = this._points[this._points.Length - 1];
                Array.Resize(ref this._points, this._points.Length + 3);
                point.x += 1f;
                this._points[this._points.Length - 3] = point;
                point.x += 1f;
                this._points[this._points.Length - 2] = point;
                point.x += 1f;
                this._points[this._points.Length - 1] = point;

                Array.Resize(ref this._modes, this._modes.Length + 1);
                this._modes[this._modes.Length - 1] = this._modes[this._modes.Length - 2];
                EnforceMode(this._points.Length - 4);

                if (this._loop)
                {
                    this._points[this._points.Length - 1] = this._points[0];
                    this._modes[this._modes.Length - 1] = this._modes[0];
                    EnforceMode(0);
                }
            }

            /// <summary>
            /// This function calculates the parameter of the closest point on the spline to a given point.
            /// </summary>
            /// <returns>
            /// The closest parameter of the point to point on the spline.
            /// </returns>
            /// <param name='point'>
            /// A given point.
            /// </param>
            /// <param name='iterations'>
            /// Defines how accurate the calculation will be. A value of 5 should be high enough for most purposes. 
            /// </param>
            /// <param name='start'>
            /// A spline parameter from 0 to 1 that specifies the lower bound for the numeric search. (default is 0.0)
            /// </param>
            /// <param name='end'>
            /// A spline parameter from 0 to 1 that specifies the upper bound for the numeric search. (default is 1.0)
            /// </param>
            /// <param name='step'>
            /// Specifies the step between two sample points on the spline for the 1st iteration. (default is 0.01) 
            /// </param>
            public float GetClosestPointParam(Vector3 point, int iterations, float start = 0, float end = 1, float step = .01f)
            {
                iterations = Mathf.Clamp(iterations, 0, 5);

                float minParam = GetClosestPointParamOnSegmentIntern(point, start, end, step);

                for (int i = 0; i < iterations; i++)
                {
                    float searchOffset = Mathf.Pow(10f, -(i + 2f));

                    start = Mathf.Clamp01(minParam - searchOffset);
                    end = Mathf.Clamp01(minParam + searchOffset);
                    step = searchOffset * .1f;

                    minParam = GetClosestPointParamOnSegmentIntern(point, start, end, step);
                }

                return minParam;
            }

            private float GetClosestPointParamOnSegmentIntern(Vector3 point, float start, float end, float step)
            {
                float minDistance = Mathf.Infinity;
                float minParam = 0f;

                for (float param = start; param <= end; param += step)
                {
                    float distance = (point - GetPosition(param)).sqrMagnitude;

                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        minParam = param;
                    }
                }

                return minParam;
            }

            private void CalculateSplineLength()
            {
                float step = 1.0f / this._interpolationAccuracy;
                Vector3 currentPos;

                float pPosX; float pPosY; float pPosZ;
                float cPosX; float cPosY; float cPosZ;

                float length = 0;

                currentPos = GetPosition(0.0f);

                pPosX = currentPos.x;
                pPosY = currentPos.y;
                pPosZ = currentPos.z;

                for (float f = step; f < (1 + step * 0.5); f += step)
                {
                    currentPos = GetPosition(f);

                    cPosX = pPosX - currentPos.x;
                    cPosY = pPosY - currentPos.y;
                    cPosZ = pPosZ - currentPos.z;

                    length += Mathf.Sqrt(cPosX * cPosX + cPosY * cPosY + cPosZ * cPosZ);

                    pPosX = currentPos.x;
                    pPosY = currentPos.y;
                    pPosZ = currentPos.z;
                }

                this._length = length;
            }

            public void Reset()
            {
                this._points = new Vector3[] 
                {
			        new Vector3(1f, 0f, 0f),
			        new Vector3(2f, 0f, 0f),
			        new Vector3(3f, 0f, 0f),
			        new Vector3(4f, 0f, 0f)
		        };

                this._modes = new BezierControlPointMode[] 
                {
			        BezierControlPointMode.Free,
			        BezierControlPointMode.Free
		        };
            }
        #endregion methods
    }
}