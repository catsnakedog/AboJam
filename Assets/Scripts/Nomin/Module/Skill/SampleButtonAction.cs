// Copyright (c) 2024 Synty Studios Limited. All rights reserved.
//
// Use of this software is subject to the terms and conditions of the Synty Studios End User Licence Agreement (EULA)
// available at: https://syntystore.com/pages/end-user-licence-agreement
//
// Sample scripts are included only as examples and are not intended as production-ready.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Synty.Interface.FantasyWarriorHUD.Samples
{
    /// <summary>
    ///     Executes a list of animator actions and toggles a list of game objects on click.
    /// </summary>
    public class SampleButtonAction : MonoBehaviour
    {
        [Header("References")]
        public Button button;
        public List<GameObject> toggleObjects;
        public GameObject activateObject;

        [Header("Parameters")]
        public List<SampleAnimatorActionData> animatorActions;
        public CoolTimer coolTimer => CoolTimer.instance;
        public float activeTime = 1f;
        public bool runOnEnable;
        public bool applyRandomRotationToActivateObject;
        public int price;
        public static Action eventSkillFail;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (button == null)
            {
                return;
            }

            button.onClick.AddListener(OnClick);
        }

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (runOnEnable)
            {
                OnClick();
            }
        }

        private void OnClick()
        {
            if (StaticData.Garu < price)
            {
                Message.instance.On("가루가 부족해요...", 2f);
                eventSkillFail.Invoke();
                return;
            }
            else StaticData.Garu -= price;
            Skill.instance.Meteor();

            StartCoroutine(C_ActivateObject());

            foreach (GameObject toggleObject in toggleObjects)
            {
                toggleObject.SetActive(!toggleObject.activeSelf);
            }

            foreach (SampleAnimatorActionData action in animatorActions)
            {
                action.Execute();
            }
        }

        private IEnumerator C_ActivateObject()
        {
            if (button != null)
            {
                button.interactable = false;
            }

            if (activateObject != null)
            {
                activateObject.SetActive(true);
                if (applyRandomRotationToActivateObject)
                {
                    activateObject.transform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
                }
            }

            yield return new WaitForSeconds(0.5f);
            while (coolTimer.RemainTime > 0) yield return new WaitForSeconds(0.1f);

            if (button != null)
            {
                button.interactable = true;
            }

            if (activateObject != null)
            {
                activateObject.SetActive(false);
            }
        }
    }
}
