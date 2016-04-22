﻿using UnityEditor;
using UnityEngine;

namespace StrayTech
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : Editor
    {
        #region const members
            private const int stepsPerCurve = 10;
            private const float directionScale = 0.5f;
            private const float handleSize = 0.15f;
            private const float pickSize = 0.06f;
        #endregion const members

        #region static members
            private static Color[] modeColors = 
            {
		        new Color(0.8f, 0.8f, 0.8f), // Free
		        new Color(0.2f, 1.0f, 0.2f), // Aligned
		        new Color(0.5f, 0.0f, 1.0f)  // Mirrored
            };
        #endregion static members

        #region members
            private BezierSpline spline;
            private Transform handleTransform;
            private Quaternion handleRotation;
            private int selectedIndex = -1;
        #endregion members

        #region methods
            private void DrawSelectedPointInspector()
            {
                GUILayout.Label("Selected Point");
                EditorGUI.BeginChangeCheck();
                Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(selectedIndex, point);
                }

                EditorGUI.BeginChangeCheck();
                BezierSpline.BezierControlPointMode mode = (BezierSpline.BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Change Point Mode");
                    spline.SetControlPointMode(selectedIndex, mode);
                    EditorUtility.SetDirty(spline);
                }
            }

            private void ShowDirections()
            {
                Handles.color = Color.green;
                Vector3 point = spline.GetPosition(0f);
                Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
                int steps = stepsPerCurve * spline.CurveCount;
                
                for (int i = 1; i <= steps; i++)
                {
                    point = spline.GetPosition(i / (float)steps);
                    Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
                }
            }

            private Vector3 ShowPoint(int index)
            {
                Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));

                float size = HandleUtility.GetHandleSize(point);

                if (index == 0)
                {
                    size *= 2f;
                }

                Handles.color = modeColors[(int)spline.GetControlPointMode(index)];

                if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.SphereCap))
                {
                    selectedIndex = index;
                    Repaint();
                }

                if (selectedIndex == index)
                {
                    EditorGUI.BeginChangeCheck();
                    point = Handles.DoPositionHandle(point, handleRotation);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(spline, "Move Point");
                        EditorUtility.SetDirty(spline);
                        spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                    }
                }
                return point;
            }
        #endregion methods

        #region monobehaviour callbacks
            public override void OnInspectorGUI()
            {
                spline = target as BezierSpline;
                EditorGUI.BeginChangeCheck();
                bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Toggle Loop");
                    EditorUtility.SetDirty(spline);
                    spline.Loop = loop;
                }

                if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
                {
                    DrawSelectedPointInspector();
                }

                if (GUILayout.Button("Add Curve"))
                {
                    Undo.RecordObject(spline, "Add Curve");
                    spline.AddCurve();
                    EditorUtility.SetDirty(spline);
                }
            }

            private void OnSceneGUI()
            {
                spline = target as BezierSpline;
                handleTransform = spline.transform;
                handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                    handleTransform.rotation : Quaternion.identity;

                Vector3 p0 = ShowPoint(0);
                
                for (int i = 1; i < spline.ControlPointCount; i += 3)
                {
                    Vector3 p1 = ShowPoint(i);
                    Vector3 p2 = ShowPoint(i + 1);
                    Vector3 p3 = ShowPoint(i + 2);

                    Handles.color = Color.gray;
                    Handles.DrawLine(p0, p1);
                    Handles.DrawLine(p2, p3);

                    Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                    p0 = p3;
                }
                //ShowDirections();
            }
        #endregion monobehaviour callbacks
    }
}