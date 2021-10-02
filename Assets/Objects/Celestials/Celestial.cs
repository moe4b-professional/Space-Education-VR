using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEngine.EventSystems;

namespace Default
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public class Celestial : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        MeshRenderer mesh;

        public float Size { get; private set; }
        public float SizeRate { get; private set; }

        [SerializeField]
        UIData _UI = default;
        public UIData UI => _UI;
        [Serializable]
        public class UIData
        {
            [SerializeField]
            CanvasGroup group = default;
            public CanvasGroup Group => group;

            [SerializeField]
            Text label = default;

            [SerializeField]
            Image border = default;

            [SerializeField]
            float transitionSpeed = 4f;

            Coroutine TransitionCoroutine;
            public Coroutine Transition(float target)
            {
                TransitionCoroutine = celestial.StartCoroutine(Procedure());
                return TransitionCoroutine;
                IEnumerator Procedure()
                {
                    if (target > 0f) group.gameObject.SetActive(true);

                    while (true)
                    {
                        group.alpha = Mathf.MoveTowards(group.alpha, target, transitionSpeed * Time.deltaTime);

                        if (group.alpha == target) break;

                        yield return new WaitForEndOfFrame();
                    }

                    if (target == 0f) group.gameObject.SetActive(false);

                    TransitionCoroutine = null;
                }
            }

            Celestial celestial;
            public void Initiate(Celestial reference)
            {
                celestial = reference;

                Transition(0f);

                Setup(celestial);
            }

            internal void Setup(Celestial celestial)
            {
                label.text = celestial.gameObject.name;
                border.rectTransform.sizeDelta = Vector2.one * 280 / celestial.SizeRate;
                border.pixelsPerUnitMultiplier = 6 * celestial.SizeRate;
            }

            public void Reset()
            {
                Transition(0f);
            }
        }

        public bool IsSelected => Selection.Current == this;

        void CalcaulateRequirements()
        {
            //Size
            {
                var scale = mesh.transform.localScale;
                Size = (scale.x + scale.y + scale.z) / 3f;
                SizeRate = 20 / Size;
            }
        }

        void OnValidate()
        {
            CalcaulateRequirements();
            UI.Setup(this);
        }

        void Start()
        {
            CalcaulateRequirements();
            UI.Initiate(this);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (IsSelected == false)
            {
                UI.Transition(0.5f);
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (IsSelected == false)
            {
                UI.Transition(0f);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsSelected == false)
            {
                Selection.Set(this);
            }
        }

        void Select()
        {
            UI.Transition(1f);
        }

        void Deselect()
        {
            UI.Reset();
        }

        //Static Utility
        public static class Selection
        {
            public static Celestial Current { get; private set; }

            public static void Set(Celestial target)
            {
                if (Current != null) Current.Deselect();

                Current = target;

                if (Current != null) Current.Select();
            }
        }
    }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
}