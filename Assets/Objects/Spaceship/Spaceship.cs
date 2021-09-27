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
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	public class Spaceship : MonoBehaviour
    {
        [SerializeField]
        InputActionAsset input = default;
        public InputActionAsset Input => input;

        [SerializeField]
        GameObject model;

        #region Move
        [Header("Move")]

        [SerializeField]
        float moveSpeed = 3.5f;

        [SerializeField]
        float moveAcceleration = 15f;
        [SerializeField]
        float moveSmoothTime;

        Vector3 moveVelocity;

        void Move()
        {
            var power = Input["Spaceship/Power"].ReadValue<float>();

            var target = transform.forward * power * moveSpeed;

            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, target, ref moveVelocity, moveSmoothTime, moveAcceleration, Time.fixedDeltaTime);
        }
        #endregion

        #region Look
        [Header("Look")]
        [SerializeField]
        float lookSpeed = 2f;

        [SerializeField]
        float lookAcceleration = 5f;
        [SerializeField]
        float lookSmoothTime;

        [SerializeField]
        float rollFactor;

        Vector3 lookDelta;
        Vector3 lookVelocity;

        void Look()
        {
            var tilt = Input["Spaceship/Tilt"].ReadValue<float>();
            var pan = Input["Spaceship/Pan"].ReadValue<float>();

#if UNITY_EDITOR
            if(Keyboard.current[Key.LeftAlt].isPressed)
            {
                tilt = 0f;
                pan = 0f;
            }
#endif

            SetJoystickModel(tilt, pan);

            var target = new Vector3()
            {
                x = tilt * lookSpeed,
                y = pan * lookSpeed,
            };

            lookDelta = Vector3.SmoothDamp(lookDelta, target, ref lookVelocity, lookSmoothTime, lookAcceleration);

            var up = Vector3.Scale(transform.up, Vector3.up);
            transform.Rotate(up, lookDelta.y * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -lookDelta.x * Time.deltaTime, Space.Self);

            {
                var angles = model.transform.localEulerAngles;
                angles.z = -Mathf.LerpUnclamped(0f, rollFactor, lookDelta.y / 180f);
                model.transform.localEulerAngles = angles;
            }

            rigidbody.rotation = transform.rotation;
        }
        #endregion

        #region Joystic
        [SerializeField]
        Transform joystick;

        void SetJoystickModel(float x, float y)
        {
            x = Mathf.Clamp(x , -1, 1);
            y = Mathf.Clamp(y, -1, 1);

            var angles = new Vector3(x, y, -y) * 15;
            var target = Quaternion.Euler(angles);

            joystick.localRotation = Quaternion.RotateTowards(joystick.localRotation, target, 180f * Time.deltaTime);
        }
        #endregion

        Rigidbody rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            input.Enable();

            input["VR/Recenter"].performed += Recenter;
        }

        void LateUpdate()
        {
            Look();
        }

        void FixedUpdate()
        {
            Move();
        }

        void Recenter(InputAction.CallbackContext context)
        {
            Google.XR.Cardboard.Api.Recenter();
        }
    }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
}