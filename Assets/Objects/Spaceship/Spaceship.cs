using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Default
{
    #pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	public class Spaceship : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset input;
        public InputActionAsset Input => input;
        
        [SerializeField]
        private GameObject model;
        
        #region Move
        [Header("Move")]

        [SerializeField]
        private float moveSpeed = 3.5f;

        [SerializeField]
        private float moveAcceleration = 15f;
        [SerializeField]
        private float moveSmoothTime;

        private Vector3 moveVelocity;

        private void Move()
        {
            var power = Input["Spaceship/Power"].ReadValue<float>();

            SampleThruster(rigidbody.velocity.magnitude / moveSpeed);

            var target = transform.forward * (power * moveSpeed);

            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, target, ref moveVelocity, moveSmoothTime, moveAcceleration, Time.fixedDeltaTime);
        }
        #endregion

        #region Look
        [Header("Look")]
        [SerializeField]
        private float lookSpeed = 2f;

        [SerializeField]
        private float lookAcceleration = 5f;
        [SerializeField]
        private float lookSmoothTime;

        [SerializeField]
        private float rollFactor;

        private Vector3 lookDelta;
        private Vector3 lookVelocity;

        private void Look()
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

            //Rotate Ship Model
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
        private Transform joystick;

        private void SetJoystickModel(float x, float y)
        {
            x = Mathf.Clamp(x , -1, 1);
            y = Mathf.Clamp(y, -1, 1);

            var angles = new Vector3(x, y, -y) * 15;
            var target = Quaternion.Euler(angles);

            joystick.localRotation = Quaternion.RotateTowards(joystick.localRotation, target, 180f * Time.deltaTime);
        }
        #endregion
        
        #region Audio
        [Header("Audio")]
        [SerializeField]
        private AudioSource thruserAudio;

        [SerializeField]
        private Vector2 thrusterAudioPitchRange;
        
        [SerializeField]
        private Vector2 thrusterAudioVolumeRange;

        void SampleThruster(float rate)
        {
            var pitch = Mathf.Lerp(thrusterAudioPitchRange.x, thrusterAudioPitchRange.y, rate);
            var volume = Mathf.Lerp(thrusterAudioVolumeRange.x, thrusterAudioVolumeRange.y, rate);

            thruserAudio.pitch = pitch;
            thruserAudio.volume = volume;
        }
        #endregion
        
        private  Rigidbody rigidbody;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            input.Enable();

            input["VR/Recenter"].performed += Recenter;
        }

        private void LateUpdate()
        {
            Look();
        }

        private void FixedUpdate()
        {
            Move();
        }

        public void Recenter(InputAction.CallbackContext context)
        {
            Google.XR.Cardboard.Api.Recenter();
        }
    }
    #pragma warning restore CS0108 // Member hides inherited member; missing new keyword
}