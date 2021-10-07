using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.EventSystems;

namespace Default
{
    #pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public class Celestial : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private string title;
        
        [SerializeField]
        private float size;

        [SerializeField]
        private AudioClip info;

        [SerializeField]
        private MeshRenderer mesh;

        [SerializeField]
        private SphereCollider collider;

        [SerializeField]
        private UIData _UI;
        public UIData UI => _UI;
        [Serializable]
        public class UIData
        {
            [SerializeField]
            CanvasGroup group;
            public CanvasGroup Group => group;

            [SerializeField]
            Text label;

            [SerializeField]
            Image border;

            [SerializeField]
            Button play;

            [SerializeField]
            float transitionSpeed = 4f;

            Coroutine transitionCoroutine;
            public Coroutine Transition(float target)
            {
                if (transitionCoroutine != null)
                    celestial.StopCoroutine(transitionCoroutine);

                transitionCoroutine = celestial.StartCoroutine(Procedure());
                return transitionCoroutine;

                IEnumerator Procedure()
                {
                    if (target > 0f) group.gameObject.SetActive(true);

                    while (true)
                    {
                        group.alpha = Mathf.MoveTowards(group.alpha, target, transitionSpeed * Time.deltaTime);

                        if (Mathf.Approximately(group.alpha, target)) break;

                        yield return new WaitForEndOfFrame();
                    }

                    if (target == 0f) group.gameObject.SetActive(false);

                    transitionCoroutine = null;
                }
            }

            internal void Validate(Celestial reference)
            {
                label.text = reference.title;
                border.rectTransform.sizeDelta = Vector2.one * 280 / reference.SizeRate;
                border.pixelsPerUnitMultiplier = 6 * reference.SizeRate;
            }

            private Celestial celestial;
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
                Core.CassettePlayer.Play(celestial.info);
            }
        }

        public float SizeRate { get; private set; }

        public bool IsSelected => Selection.Current == this;

        public static SpaceVRCore Core => SpaceVRCore.Instance;

        private void Validate()
        {
            //Size
            {
                mesh.transform.localScale = Vector3.one * size;
                collider.radius = size / 2f;
                SizeRate = 20 / size;
            }

            UI.Validate(this);
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += Delay;
            void Delay()
            {
                EditorApplication.delayCall -= Delay;

                if (this == null) return;

                Validate();
            }
#endif
        }

        private void Start()
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

        private void Select()
        {
            UI.Select();
        }

        private void Deselect()
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