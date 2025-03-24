using Externals.Serializer;
using GameCore.Http.Auth;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager
{
	[SerializeField] private string _url;
	[SerializeField] private string _userId;
	private MemoryPackDataSerializer _serializer;

	public LoginManager()
	{
		_serializer = MemoryPackDataSerializer.Instance;
	}

	public IEnumerable Req_Auth()
	{
		const string path = "auth/auth";

		var req = new REQ_Auth
		{
			UserId = _userId
		};

		using (UnityWebRequest request = UnityWebRequest.Post(Path.Combine(_url, path)))
		{
			//스트링으로 넘기면 json 구성이 깨지기 때문에 byte로 변환 후 파일로 업로드해준다
			byte[] jsonToSend = new UTF8Encoding().GetBytes(parameter);
			request.uploadHandler = new UploadHandlerRaw(jsonToSend);
			//json 헤더 추가
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			Debug.Log(request.result);
		}
	}

	public bool Req_CreatePlayer()
	{
		using (UnityWebRequest request = UnityWebRequest.Get("https://my-website.com"))
		{
			yield return request.Send();
			Debug.Log("Server responded: " + request.downloadHandler.text);
		}

		return true;
	}

	public bool Req_Login()
	{
		using (UnityWebRequest request = UnityWebRequest.Get("https://my-website.com"))
		{
			yield return request.Send();
			Debug.Log("Server responded: " + request.downloadHandler.text);
		}

		return true;
	}
}
