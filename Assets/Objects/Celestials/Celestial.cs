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
        string title = default;

        [SerializeField]
        float size;

        [SerializeField]
        MeshRenderer mesh;

        [SerializeField]
        SphereCollider collider;

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
            Button play = default;

            [SerializeField]
            float transitionSpeed = 4f;

            Coroutine TransitionCoroutine;
            public Coroutine Transition(float target)
            {
                if (TransitionCoroutine != null)
                    celestial.StopCoroutine(TransitionCoroutine);

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

            internal void Validate(Celestial celestial)
            {
                label.text = celestial.title;
                border.rectTransform.sizeDelta = Vector2.one * 280 / celestial.SizeRate;
                border.pixelsPerUnitMultiplier = 6 * celestial.SizeRate;
            }

            Celestial celestial;
            public void Start(Celestial reference)
            {
                celestial = reference;

                group.gameObject.SetActive(false);
                play.gameObject.SetActive(false);

                Validate(celestial);

                play.onClick.AddListener(OnPlay);
            }

            public void Select()
            {
                play.gameObject.SetActive(true);

                Transition(1f);
            }

            public void Deselect()
            {
                play.gameObject.SetActive(false);

                Transition(0f);
            }

            void OnPlay()
            {
                Debug.Log("Play");
            }
        }

        public float SizeRate { get; private set; }

        public bool IsSelected => Selection.Current == this;

        void Validate()
        {
            //Size
            {
                mesh.transform.localScale = Vector3.one * size;
                collider.radius = size / 2f;
                SizeRate = 20 / size;
            }

            UI.Validate(this);
        }

        void OnValidate()
        {
            Validate();
        }

        void Start()
        {
            Validate();
            UI.Start(this);
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
            UI.Select();
        }

        void Deselect()
        {
            UI.Deselect();
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