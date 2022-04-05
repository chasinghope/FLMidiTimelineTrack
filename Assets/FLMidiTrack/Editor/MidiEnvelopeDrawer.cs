using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Chasing.Midi.Timeline
{
    [CustomPropertyDrawer(typeof(MidiEnvelope), true)]
    sealed class MidiEnvelopeDrawer : PropertyDrawer
    {
        private const float GraphHeight = 40f;

        static Color backgroundColor => EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.45f, 0.45f, 0.45f);
        static Color highlightColor => EditorGUIUtility.isProSkin ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.5f, 0.5f, 0.5f);
        static Color guideColor => EditorGUIUtility.isProSkin ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.56f, 0.56f, 0.56f);
        static Color LineColor => EditorGUIUtility.isProSkin ? new Color(0.6f, 0.9f, 0.4f) : new Color(0.4f, 0.9f, 0.2f);
        static Color noteOnColor => new Color(255/255f, 165/255f, 0/255f);
        static Color noteOffColor = new Color(108/255f, 160/255f, 220/255f);

        static readonly GUIContent[] _adsrLabels = 
        {
            new GUIContent("A"), new GUIContent("D"),
            new GUIContent("S"), new GUIContent("R")
        };

        static Vector3[] mLineVerts = new Vector3[2];
        static Vector3[] mGraphVerts = new Vector3[6];

        #region Static Method

        public static float GetHeight()
        {
            float line = EditorGUIUtility.singleLineHeight;
            float space = EditorGUIUtility.standardVerticalSpacing;
            if (!EditorGUIUtility.wideMode) line *= 2;
            return line + GraphHeight + space * 3;
        }


        #endregion

        #region PropertyDrawer Override

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Head control ID: Used to determine the control IDs.
            var id0 = GUIUtility.GetControlID(FocusType.Passive);

            // Envelope parameters (ADSR)
            position = DrawEnvelopeParameterFields(position, property, label);

            // Envelope graph
            GUI.BeginGroup(position);
            DrawGraph(RetrieveEnvelope(property), position.width, position.height, id0 + 2);
            GUI.EndGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight();
        }


        #endregion

        #region Instance Method

        private Rect DrawEnvelopeParameterFields(Rect rect, SerializedProperty prop, GUIContent label)
        {
            SerializedProperty itr = prop.Copy();

            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, label);
            if (EditorGUIUtility.wideMode)
            {
                rect.x += EditorGUIUtility.labelWidth;
                rect.width -= EditorGUIUtility.labelWidth;
            }
            else
            {
                rect.y += rect.height;

                EditorGUI.indentLevel++;
                rect = EditorGUI.IndentedRect(rect);
                EditorGUI.indentLevel--;
            }

            Rect r = rect;
            r.width = (r.width - 6) / 4;

            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 12;

            for (var i = 0; i < 4; i++)
            {
                itr.Next(true);

                // Element field
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(r, itr, _adsrLabels[i]);

                // Apply the value constraint.
                if (EditorGUI.EndChangeCheck())
                    if (i == 2)
                        itr.floatValue = Mathf.Clamp01(itr.floatValue); // S
                    else
                        itr.floatValue = Mathf.Max(0, itr.floatValue); // ADR

                // Move to the next field.
                r.x += r.width + 2;
            }

            // Recover the original label width.
            EditorGUIUtility.labelWidth = originalLabelWidth;

            // Calculate the graph position.
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = GraphHeight;
            return rect;
        }



        private MidiEnvelope RetrieveEnvelope(SerializedProperty prop)
        {
            return new MidiEnvelope
            {
                attack = prop.FindPropertyRelative("attack").floatValue,
                decay = prop.FindPropertyRelative("decay").floatValue,
                sustain = prop.FindPropertyRelative("sustain").floatValue,
                release = prop.FindPropertyRelative("release").floatValue
            };
        }


        private void DrawAALine(float x0, float y0, float x1, float y1)
        {
            mLineVerts[0].x = x0;
            mLineVerts[0].y = y0;
            mLineVerts[1].x = x1;
            mLineVerts[1].y = y1;
            Handles.DrawAAPolyLine(mLineVerts);
        }


        private void DrawGraph(MidiEnvelope env, float width, float height, int controlID)
        {
            const float scale = 2;

            // Time parameters
            var t1 = scale * env.AttackTime;
            var t2 = t1 + scale * env.DecayTime;
            var t3 = t2 + scale * 0.2f;
            var t4 = t3 + scale * env.ReleaseTime;

            // Position parameters
            var x1 = 1 + width * t1;
            var x2 = 1 + width * t2;
            var x3 = 1 + width * t3;
            var x4 = 1 + width * t4;
            var sus_y = (1 - env.SustainLevel) * (height - 2) + 1;

            // ADSR graph vertices
            mGraphVerts[0] = new Vector3(1, height);
            mGraphVerts[1] = new Vector3(x1, 1);
            mGraphVerts[2] = new Vector3(x2, sus_y);
            mGraphVerts[3] = new Vector3(x3, sus_y);
            mGraphVerts[4] = new Vector3(x4, height - 1);
            mGraphVerts[5] = new Vector3(width, height - 1);

            // Background
            EditorGUI.DrawRect(new Rect(0, 0, width, height), backgroundColor);

            // Guide elements
            var focus = GUIUtility.keyboardControl;

            if (focus == controlID)
                EditorGUI.DrawRect(new Rect(0, 0, x1, height), highlightColor);
            else if (focus == controlID + 1)
                EditorGUI.DrawRect(new Rect(x1, 0, x2 - x1, height), highlightColor);
            else if (focus == controlID + 2)
                EditorGUI.DrawRect(new Rect(0, sus_y, width, height), highlightColor);
            else if (focus == controlID + 3)
                EditorGUI.DrawRect(new Rect(x3, 0, x4 - x3, height), highlightColor);

            Handles.color = guideColor;
            DrawAALine(x1, 0, x1, height);
            DrawAALine(x2, 0, x2, height);
            DrawAALine(x3, 0, x3, height);
            DrawAALine(x4, 0, x4, height);
            DrawAALine(0, sus_y, width, sus_y);
            Handles.color = noteOnColor;
            DrawAALine(x1, 1, x1, height);
            Handles.color = noteOffColor;
            DrawAALine(x3, sus_y, x3, height);

            // ADSR graph
            Handles.color = LineColor;
            Handles.DrawAAPolyLine(mGraphVerts);
        }

        #endregion


    }
}
