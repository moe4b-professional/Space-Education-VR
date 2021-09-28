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

namespace Default
{
	[RequireComponent(typeof(LineRenderer))]
	public class OrbitLine : MonoBehaviour
	{
        [SerializeField]
        Transform origin = default;

        public Vector3 GetOriginPosition()
        {
            if (origin == null)
                return Vector3.zero;

            return origin.position;
        }

        [SerializeField]
        Transform target = default;

        [SerializeField]
        int segments = default;

        [SerializeField]
        Color color = Color.white;

        MaterialPropertyBlock block;

        [SerializeField]
        LineRenderer line;

        void Reset()
        {
            origin = null;
            target = transform;

            line = GetComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;

            Create();
        }

        void OnValidate()
        {
            Create();
        }

        void Start()
        {
            Create();
        }

        void Create()
        {
            var origin = GetOriginPosition();

            block = new MaterialPropertyBlock();
            block.SetColor("_Color", color);
            line.SetPropertyBlock(block);

            var radius = Vector3.Distance(origin, target.position);

            line.positionCount = segments;

            for (int i = 0; i < segments; i++)
            {
                var rate = (i) / 1f / (segments - 1);
                var angle = Mathf.Lerp(0f, 360f, rate);
                var rotation = Quaternion.Euler(Vector3.up * angle);
                var direction = rotation * transform.forward;
                var point = origin + direction * radius;
                point = line.transform.InverseTransformPoint(point);

                line.SetPosition(i, point);
            }
        }

#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(OrbitLine))]
		public class Inspector : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Create"))
                    base.targets.Cast<OrbitLine>().ForAll(x => x.Create());
            }
        }
#endif
	}
}