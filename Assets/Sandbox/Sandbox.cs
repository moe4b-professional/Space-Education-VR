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
    public class Sandbox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [RuntimeInitializeOnLoadMethod]
        public static void OnLoad()
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer Enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Pointer Exit");
        }
    }
}