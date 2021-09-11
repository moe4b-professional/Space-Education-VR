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

        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;
        public Vector3 Up => transform.up;

        [SerializeField]
        GameObject model;

        #region Move
        [Header("Move")]

        [SerializeField]
        float moveSpeed = 3.5f;

        [SerializeField]
        float moveAcceleration = 15f;

        Vector3 moveVelocity;

        void Move()
        {
            var power = Input["Spaceship/Power"].ReadValue<float>();

            var target = Forward * power * moveSpeed;

            moveVelocity = rigidbody.velocity;
            moveVelocity = Vector3.MoveTowards(moveVelocity, target, moveAcceleration * Time.deltaTime);

            rigidbody.velocity = moveVelocity;
        }
        #endregion

        #region Look
        [Header("Look")]
        [SerializeField]
        float lookSpeed = 2f;

        [SerializeField]
        float lookAcceleration = 5f;

        [SerializeField]
        float rollFactor;

        Vector3 lookDelta;

        void Look()
        {
            var tilt = Input["Spaceship/Tilt"].ReadValue<float>();
            var pan = Input["Spaceship/Pan"].ReadValue<float>();

            SetJoystickModel(tilt, pan);

            var target = new Vector3()
            {
                x = tilt * lookSpeed,
                y = pan * lookSpeed,
            };

            lookDelta = Vector3.MoveTowards(lookDelta, target, lookAcceleration * Time.deltaTime);

            transform.Rotate(Vector3.up, lookDelta.y * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -lookDelta.x * Time.deltaTime, Space.Self);

            {
                var angles = model.transform.localEulerAngles;
                angles.z = -Mathf.LerpUnclamped(0f, rollFactor, lookDelta.y / 180f);
                model.transform.localEulerAngles = angles;
            }

            rigidbody.rotation = transform.rotation;
        }

        void SetJoystickModel(float x, float y)
        {
            var angles = new Vector3(x, y , -y);
            angles *= 15;
            joystick.localEulerAngles = angles;
        }
        #endregion

        [SerializeField]
        Transform joystick;

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
            Move();
        }

        void Recenter(InputAction.CallbackContext context)
        {
            Google.XR.Cardboard.Api.Recenter();
        }
    }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
}