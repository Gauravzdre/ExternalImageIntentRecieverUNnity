using UnityEngine;
using UnityEngine.UI;

namespace BlackKnight {
	public class Test : MonoBehaviour {

		[SerializeField] RawImage img;

		/// <summary>
		/// Subscribe the Action<Texture2D> event in start 
		/// To Revoke all subscriptions of this event use ExternalIntentReciever.RevokeSubscriptions()
		/// </summary>
		private void Start() {
			ExternalIntentReciever.OnGetImage += ExternalIntentReciever_OnGetImage;
		}

		/// <summary>
		/// this is just applying the Texture2D to the Raw Image
		/// </summary>
		/// <param name="tex">Recieved Texture2D </param>
		private void ExternalIntentReciever_OnGetImage(Texture2D tex) {
			Debug.Log(tex == null);
			img.texture = tex;
		}
	}
}
