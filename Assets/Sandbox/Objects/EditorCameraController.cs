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

using UnityEngine.InputSystem;

namespace Default
{
	public class EditorCameraController : MonoBehaviour
	{
		[SerializeField]
		float sensitivity;

		[SerializeField]
        Key modifier = Key.LeftAlt;

        Keyboard Keyboard => Keyboard.current;
        Mouse mouse => Mouse.current;

        void Update()
        {
            if (Keyboard[modifier].wasPressedThisFrame)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Keyboard[modifier].wasReleasedThisFrame)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (Keyboard[modifier].isPressed)
            {
                var delta = new Vector2()
                {
                    x = mouse.delta.x.ReadValue(),
                    y = mouse.delta.y.ReadValue(),
                };

                delta *= sensitivity * Time.deltaTime;

                var angles = transform.localEulerAngles;

                angles.x -= delta.y;
                angles.y += delta.x;

                angles.x = ClampAngle(angles.x, -60, 60);
                angles.y = ClampAngle(angles.y, -140, 140);

                transform.localEulerAngles = angles;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            float start = (min + max) * 0.5f - 180;
            float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
            min += floor;
            max += floor;
            return Mathf.Clamp(angle, min, max);
        }
    }
}