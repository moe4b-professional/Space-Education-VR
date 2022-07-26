using UnityEngine;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

namespace Default
{
	public class CassettePlayerIconSwitcher : MonoBehaviour
	{
		[SerializeField]
		GameObject on;

		[SerializeField]
		GameObject off;

		CassettePlayer player => SpaceVRCore.Instance.CassettePlayer;

		void Update()
        {
			on.SetActive(player.Current != null);
			off.SetActive(player.Current == null);
		}
    }
}