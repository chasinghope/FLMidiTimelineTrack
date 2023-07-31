using Chasing.Midi.Timeline;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Chasing.TrackWindow
{
    public class SelectSlot : MonoBehaviour
    {
        [SerializeField] GameObject PropertyPanel;
        [SerializeField] Text selectCountText;

        [Header("自定义数据")]
        [SerializeField] TMP_Dropdown noteTypeDropDown;

        public List<Slot> selectList = new List<Slot>();

        public CustomExpandData expandData;


        public bool isCtrlOn;

        private void Awake()
        {
            noteTypeDropDown.onValueChanged.AddListener(NoteTypeDropDown_OnValueChaned);
            HidePanel();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCtrlOn = true;
            }
            if(Input.GetKeyUp (KeyCode.LeftControl))
            {
                isCtrlOn = false;
                //多选

                if (selectList.Count == 0)
                {
                    HidePanel();
                }
                else
                {
                    ShowPanel();
                }

            }
        }

        private void NoteTypeDropDown_OnValueChaned(int arg0)
        {
            expandData.noteType = (NoteType)arg0;
            WriteNewProperty();
        }



        private void WriteNewProperty()
        {
            foreach (Slot slot in selectList)
            {
                slot.SetNewCustomData(expandData);
            }
        }

        private void RefreshUI()
        {

            noteTypeDropDown.value = (int)expandData.noteType;
        }


        private void RefreshSelectCount()
        {
            this.selectCountText.text = $"选中Slot数量: {selectList.Count}";
        }


        public void SelectSlotUnit(Slot slot)
        {
            if(isCtrlOn == false)
            {
                foreach (var item in selectList)
                {
                    item.SelecteSlot();
                }
                selectList.Clear();
            }

            if(selectList.Contains(slot))
            {
                slot.SelecteSlot();
                selectList.Remove(slot);
            }
            else
            {
                slot.SelecteSlot();
                selectList.Add(slot);
            }

            if(isCtrlOn == false)
            {

                if(selectList.Count == 0)
                {
                    HidePanel();
                }
                else
                {
                    ShowPanel();
                }

            }

        }

        private void ShowPanel()
        {
            PropertyPanel.gameObject.SetActive(true);

            if (selectList.Count != 1)
            {
                expandData = new CustomExpandData();
                return;
            }
            expandData = selectList[0].GetCustomData();
            RefreshUI();
            RefreshSelectCount();
        }

        private void HidePanel()
        {
            PropertyPanel.gameObject.SetActive(false);
        }
    }
}