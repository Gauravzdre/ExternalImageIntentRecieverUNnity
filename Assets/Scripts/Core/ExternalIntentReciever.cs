using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

namespace BlackKnight {
	/// <summary>
	/// Handles the External intents sent by other apps 
	/// </summary>
	public class ExternalIntentReciever : MonoBehaviour {
		const string TAG = "ExternalIntent";

		public static event Action<Texture2D> OnGetImage;

		string m_LastPath;

		private void Start() {
#if UNITY_ANDROID && !UNITY_EDITOR
			ExtractData();
#endif
		}

		void OnApplicationFocus(bool focus) {
#if !UNITY_EDITOR
			if (focus)
				ExtractData();
#endif
		}

		// TODO: Invoke from somewhere
		public static void RevokeSubscriptions() {
			foreach (var i in OnGetImage.GetInvocationList())
				OnGetImage -= (Action<Texture2D>)i;
		}


		public void ExtractData() {
			AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");//can be used to extract data from notification or deep linking

			string path = string.Empty;
			AndroidJavaObject externalIntent = new AndroidJavaObject("com.example.externalintent.ExternalIntent");
			path = externalIntent.Call<string>("getRealPathFromURI", new object[] { currentActivity });

			string action = intent.Call<string>("getAction");

			// If the app opened using the MAIN action, the app was opened using the icon
			// and this is not a notification or deep link action
			if (action == "android.intent.action.MAIN") {
				Debug.Log(TAG + " : Recognised android.intent.action.MAIN");
				return;
			}

			// If there is no path data in the intent, we ignore it
			if (string.IsNullOrEmpty(path)) {
				Debug.Log(TAG + " : No action path recognised");
				return;
			}

			// If the path data is the same as the previously detected one, we ignore it
			if (m_LastPath.Equals(path)) {
				Debug.Log(TAG + " : Action path repeat. Ignoring.");
				return;
			}

			Debug.Log(TAG + " : Action Path: " + path);

			// If the path points to a cached image
			if (path.Contains("external_files") || path.Contains("external/file")) {
				Debug.Log(TAG + " : Cached image detected in action");

				GetCachedImage(path, res => {
					if (res != null)
						OnGetImage?.Invoke(res);
					else
						OnGetImage?.Invoke(null);
				});

			}
			// Else we get the image from the remote URL
			else {
				Runner.New(GetOnlineImage(path, tex => {
					if (tex != null)
						OnGetImage?.Invoke(tex);
					else
						OnGetImage?.Invoke(null);
				})).Run();
			}
			m_LastPath = path;
		}

		IEnumerator GetOnlineImage(string path, Action<Texture2D> callback) {
			UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
			yield return request.SendWebRequest();

			if (request.error != null) {
				Debug.LogError(TAG + " : Error fetching image: " + request.error);
				callback(null);
				yield break;
			}

			Texture2D tex = new Texture2D(2, 2);
			var imageBytes = request.downloadHandler.data;
			if (tex.LoadImage(imageBytes))
				callback(tex);
			else
				callback(null);
		}

		void GetCachedImage(string path, Action<Texture2D> callback) {
			path = path.Replace("/external_files", "/sdcard");
			Debug.Log(TAG + " : Trying to load cached image from: " + path);

			if (!File.Exists(path)) {
				Debug.LogError(TAG + " : No such file: " + path);
				callback?.Invoke(null);
				return;
			}

			byte[] imageData = File.ReadAllBytes(path);
			if (imageData == null) {
				Debug.LogError(TAG + " : Could not read file bytes: " + path);
				callback(null);
				return;
			}

			Texture2D tex = new Texture2D(2, 2);
			if (tex.LoadImage(imageData))
				callback(tex);
			else
				callback(null);
		}
	}
}