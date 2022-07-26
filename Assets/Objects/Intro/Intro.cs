using UnityEngine;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using UnityEngine.InputSystem;

namespace Default
{
	public class Intro : MonoBehaviour
	{
		[SerializeField]
		InputActionAsset input;

        [SerializeField]
        GameObject ship;

        void Start()
        {
            input["UI/Click"].performed += Clicked;
        }

        void Clicked(InputAction.CallbackContext context)
        {
            gameObject.SetActive(false);
            ship.SetActive(true);
        }
    }
}