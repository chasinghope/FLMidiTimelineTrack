using UnityEditor;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    [CustomEditor(typeof(MidiSignalReceiver))]
    sealed class MidiSignalReceiverEditor : Editor
    {
        SerializedProperty mNoteFilter;
        SerializedProperty mNoteOnEvent;
        SerializedProperty mNoteOffEvent;

        static readonly GUIContent _labelNoteOctave = new GUIContent("Note/Octave");

        void OnEnable()
        {
            mNoteFilter = serializedObject.FindProperty("noteFilter");
            mNoteOnEvent = serializedObject.FindProperty("noteOnEvent");
            mNoteOffEvent = serializedObject.FindProperty("noteOffEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(mNoteFilter, _labelNoteOctave);
            EditorGUILayout.PropertyField(mNoteOnEvent);
            EditorGUILayout.PropertyField(mNoteOffEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
