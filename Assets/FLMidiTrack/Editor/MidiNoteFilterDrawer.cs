using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    [CustomPropertyDrawer(typeof(MidiNoteFilter), true)]
    sealed class MidiNoteFilterDrawer : PropertyDrawer
    {
        private static readonly int[] mNoteValues = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        private static readonly GUIContent[] mNoteLabels =
        {
            new GUIContent("All"),
            new GUIContent("C" ),   new GUIContent("C#"),   new GUIContent("D" ),
            new GUIContent("D#"),   new GUIContent("E" ),   new GUIContent("F" ),
            new GUIContent("F#"),   new GUIContent("G" ),   new GUIContent("G#"),
            new GUIContent("A" ),   new GUIContent("A#"),   new GUIContent("B" )
        };

        private static readonly int[] mOctaveValues = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        private static readonly GUIContent[] mOctaveLabels =
        {
            new GUIContent("All"),
            new GUIContent("-2"),   new GUIContent("-1"),   new GUIContent("0" ),
            new GUIContent("1" ),   new GUIContent("2" ),   new GUIContent("3" ),
            new GUIContent("4" ),   new GUIContent("5" ),   new GUIContent("6" ),
            new GUIContent("7" ),   new GUIContent("8" )
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            position.x += EditorGUIUtility.labelWidth;
            position.width = (position.width - EditorGUIUtility.labelWidth - 4) / 2;

            //Note Dropdown
            SerializedProperty note = property.FindPropertyRelative("note");
            EditorGUI.BeginChangeCheck();
            int index = EditorGUI.IntPopup(position, note.enumValueIndex, mNoteLabels, mNoteValues);
            if (EditorGUI.EndChangeCheck())
                note.enumValueIndex = index;

            position.x += position.width + 4;

            //Octave dropdown
            SerializedProperty octave = property.FindPropertyRelative("octave");
            EditorGUI.BeginChangeCheck();
            index = EditorGUI.IntPopup(position, octave.enumValueIndex, mOctaveLabels, mOctaveValues);
            if (EditorGUI.EndChangeCheck())
                octave.enumValueIndex = index;

        }

    }
}
