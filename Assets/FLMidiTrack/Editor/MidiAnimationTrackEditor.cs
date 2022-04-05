using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;


namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// 提供UI来编辑midi control
    /// </summary>
    [CustomEditor(typeof(MidiAnimationTrack))]
    sealed class MidiAnimationTrackEditor : Editor
    {
        SerializedProperty mControls;

        #region Unity

        private void OnEnable()
        {
            mControls = serializedObject.FindProperty("template.controls");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < mControls.arraySize; i++)
            {
                CoreEditorUtils.DrawSplitter();

                string title = $"Control Element {i+1}";
                SerializedProperty control = mControls.GetArrayElementAtIndex(i);
                SerializedProperty enabled = control.FindPropertyRelative("enabled");

                bool toggle = CoreEditorUtils.DrawHeaderToggle
                        (title, control, enabled, pos => OnContextClick(pos, i));

                if (!toggle) continue;

                using (new EditorGUI.DisabledScope(!enabled.boolValue))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(control);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (EditorGUI.EndChangeCheck())
                TimelineEditor.Refresh(RefreshReason.ContentsModified);


            if (mControls.arraySize > 0)
                CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Control Element"))
                AppendDefaultMidiControl();
        }


        #endregion

        #region Instance Method

        private void AppendDefaultMidiControl()
        {
            // Expand the array via SerializedProperty.
            int index = mControls.arraySize;
            mControls.InsertArrayElementAtIndex(index);

            SerializedProperty prop = mControls.GetArrayElementAtIndex(index);
            prop.isExpanded = true;

            serializedObject.ApplyModifiedProperties();

            // Set a new control instance.
            MidiAnimationTrack track = (MidiAnimationTrack)target;
            MidiControl[] controls = track.template.controls;
            Undo.RecordObject(track, "Add MIDI Control");
            controls[controls.Length - 1] = new MidiControl();
        }

        private void CopyControl(MidiControl src, MidiControl dst, bool updateGuid)
        {
            // Copy MidiControl members.
            // Is there any smarter way to do this?
            dst.enabled = src.enabled;
            dst.mode = src.mode;
            dst.noteFilter = src.noteFilter;
            dst.envelope = src.envelope;
            dst.curve = new AnimationCurve(src.curve.keys);
            dst.ccNumber = src.ccNumber;

            if (updateGuid)
            {
                // Copy targetComponent as a new reference.
                var guid = GUID.Generate().ToString();
                dst.targetComponent.exposedName = guid;
                var resolver = serializedObject.context as IExposedPropertyTable;
                resolver?.SetReferenceValue(guid, src.targetComponent.Resolve(resolver));
            }
            else
                // Simply copy targetComponent.
                dst.targetComponent = src.targetComponent;
        }



        private void OnContextClick(Vector2 pos, int index)
        {
            var menu = new GenericMenu();

            // "Move Up"
            if (index == 0)
                menu.AddDisabledItem(Labels.MoveUp);
            else
                menu.AddItem(Labels.MoveUp, false, () => OnMoveControl(index, index - 1));

            // "Move Down"
            if (index == mControls.arraySize - 1)
                menu.AddDisabledItem(Labels.MoveDown);
            else
                menu.AddItem(Labels.MoveDown, false, () => OnMoveControl(index, index + 1));

            // "Reset" / "Remove"
            menu.AddSeparator(string.Empty);
            menu.AddItem(Labels.Reset, false, () => OnResetControl(index));
            menu.AddItem(Labels.Remove, false, () => OnRemoveControl(index));

            // "Copy" / "Paste"
            menu.AddSeparator(string.Empty);
            menu.AddItem(Labels.Copy, false, () => OnCopyControl(index));
            menu.AddItem(Labels.Paste, false, () => OnPasteControl(index));

            // Show the drop down.
            menu.DropDown(new Rect(pos, Vector2.zero));
        }

        private void OnMoveControl(int src, int dst)
        {
            serializedObject.Update();
            mControls.MoveArrayElement(src, dst);
            serializedObject.ApplyModifiedProperties();
            // We don't need to refresh the timeline in this case.
            // TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        private void OnResetControl(int index)
        {
            var track = (MidiAnimationTrack)target;
            Undo.RecordObject(track, "Reset MIDI Control");
            track.template.controls[index] = new MidiControl();
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        private void OnRemoveControl(int index)
        {
            serializedObject.Update();
            mControls.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        private void OnCopyControl(int index)
        {
            var track = (MidiAnimationTrack)target;
            Undo.RecordObject(track, "Copy MIDI Control");
            CopyControl(track.template.controls[index], mClipborad, false);
        }

        private void OnPasteControl(int index)
        {
            var track = (MidiAnimationTrack)target;
            Undo.RecordObject(track, "Paste MIDI Control");
            CopyControl(mClipborad, track.template.controls[index], true);
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }


        #endregion

        #region Static


        static class Labels
        {
            public static readonly GUIContent MoveUp = new GUIContent("Move Up");
            public static readonly GUIContent MoveDown = new GUIContent("Move Down");
            public static readonly GUIContent Reset = new GUIContent("Reset");
            public static readonly GUIContent Remove = new GUIContent("Remove");
            public static readonly GUIContent Copy = new GUIContent("Copy");
            public static readonly GUIContent Paste = new GUIContent("Paste");
        }

        static MidiControl mClipborad = new MidiControl();

        #endregion

    }
}
