using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    [CustomPropertyDrawer(typeof(MidiControl), true)]
    sealed class MidiControlDrawer : PropertyDrawer
    {
        private Dictionary<string, MidiControlInternalDrawer> mDrawer = new Dictionary<string, MidiControlInternalDrawer>();

        private MidiControlInternalDrawer GetCachedDrawer(SerializedProperty property)
        {
            MidiControlInternalDrawer drawer;

            var path = property.propertyPath;
            mDrawer.TryGetValue(path, out drawer);

            if (drawer == null)
            {
                // No instance was found witht the given path,
                // so create a new instance for it.
                drawer = new MidiControlInternalDrawer(property);
                mDrawer[path] = drawer;
            }
            return drawer;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MidiControlInternalDrawer drawer = GetCachedDrawer(property);

            drawer.SetRect(position);
            drawer.DrawDetail();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetCachedDrawer(property).GetHeigth();
        }

    }


    sealed class MidiControlInternalDrawer
    {
        private SerializedProperty mEnabled;
        private SerializedProperty mMode;
        private SerializedProperty mNoteFilter;
        private SerializedProperty mEnvelope;
        private SerializedProperty mCurve;
        private SerializedProperty mCCNumber;
        private SerializedProperty mTargetComponent;


        static readonly GUIContent mLabelControlMode = new GUIContent("Control Mode");
        static readonly GUIContent mLabelCCNumber = new GUIContent("CC Number");
        static readonly GUIContent mLabelTarget = new GUIContent("Target");
        static readonly GUIContent mLabelNoteOctave = new GUIContent("Note/Octave");

        private Rect mBaseRect;
        private Rect mRect;

        public MidiControlInternalDrawer(SerializedProperty property)
        {
            mEnabled = property.FindPropertyRelative("enabled");
            mMode = property.FindPropertyRelative("mode");
            mNoteFilter = property.FindPropertyRelative("noteFilter");
            mEnvelope = property.FindPropertyRelative("envelope");
            mCurve = property.FindPropertyRelative("curve");
            mCCNumber = property.FindPropertyRelative("ccNumber");
            mTargetComponent = property.FindPropertyRelative("targetComponent");
        }

        public void DrawDetail()
        {

            mRect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(mRect, mNoteFilter, mLabelNoteOctave);
            MoveRectToNextLine();

            EditorGUI.PropertyField(mRect, mMode, mLabelControlMode);
            MoveRectToNextLine();

            switch ((MidiControl.Mode)mMode.enumValueIndex)
            {
                case MidiControl.Mode.NoteEnvelope:
                    Rect r = mRect;
                    r.height = MidiEnvelopeDrawer.GetHeight();
                    EditorGUI.PropertyField(mRect, mEnvelope);
                    mRect.y += r.height;
                    break;
                case MidiControl.Mode.NoteCurve:
                    EditorGUI.PropertyField(mRect, mCurve);
                    MoveRectToNextLine();
                    break;
                case MidiControl.Mode.CC:
                    EditorGUI.PropertyField(mRect, mCCNumber, mLabelCCNumber);
                    MoveRectToNextLine();
                    break;
                default:
                    break;
            }

            EditorGUI.PropertyField(mRect, mTargetComponent, mLabelTarget);
            MoveRectToNextLine();
        }

        public void SetRect(Rect position)
        {
            mRect = mBaseRect = position;
        }


        private void MoveRectToNextLine()
        {
            mRect.y += EditorGUIUtility.singleLineHeight + 2;
        }

        public float GetHeigth()
        {
            return mRect.y - mBaseRect.y;
        }
    }
}