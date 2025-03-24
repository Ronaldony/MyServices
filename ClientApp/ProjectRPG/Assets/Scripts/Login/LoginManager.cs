using Externals.Serializer;
using GameCore.Http.Auth;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _noticeText;
	[SerializeField] private string _url;
    [SerializeField] private string _loadScene;
    private PlayerDataManager _playerDataManager;

    private void Start()
    {
        _playerDataManager = PlayerDataManager.Instance;
    }

    /// <summary>
    /// Request auth.
    /// </summary>
	public IEnumerator WebReq_Auth(string userId)
	{
        _noticeText.text = "Requesting log in";

        const string path = "auth/auth";

		var postData = MemoryPackDataSerializer.Serialize(new REQ_Auth
        {
            UserId = userId
        });

        using (var request = new UnityWebRequest($"{_url}/{path}", UnityWebRequest.kHttpVerbPOST))
		{
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-memorypack");

            // request.
            yield return request.SendWebRequest();

            Debug.Log($"msg: {request.result}, error: {request.error}, code: {request.responseCode}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 응답 가져오기.
                byte[] responseData = request.downloadHandler.data;
                var resData = MemoryPackDataSerializer.Deserialize<RES_Auth>(responseData);

                if (resData.PlayerDataDict != null)
                {
                    // 기존 플레이어
                    var pair = resData.PlayerDataDict.First();

                    _playerDataManager.SetPlayer(userId, resData.Uid, pair.Key, pair.Value);
                    _playerDataManager.SetAuthProcess(resData.AccessToken, resData.RefreshToken, resData.RefrshTokenExpire);

                    StartCoroutine(WebReq_Login(_playerDataManager.UserId, _playerDataManager.Uid, _playerDataManager.Pid, _playerDataManager.AccessToken));
                }
                else
                {
                    // 새로운 플레이어
                    _playerDataManager.SetPlayer(userId, resData.Uid);
                    _playerDataManager.SetAuthProcess(resData.AccessToken, resData.RefreshToken, resData.RefrshTokenExpire);

                    StartCoroutine(WebReq_CreatePlayer(_playerDataManager.UserId, _playerDataManager.Uid, _playerDataManager.AccessToken));
                }
            }
            else
            {
                _noticeText.text = "Failed to authentice";

                yield return new WaitForSeconds(5f);

                _noticeText.text = string.Empty;
            }
		}
	}

    /// <summary>
    /// Request create player.
    /// </summary>
    public IEnumerator WebReq_CreatePlayer(string userId, string uid, string accessToken)
    {
        const string path = "player/create";

        var postData = MemoryPackDataSerializer.Serialize(new REQ_CreatePlayer
        {
            UserId = userId,
            AccessToken = accessToken,
            Uid = uid,
        });

        using (var request = new UnityWebRequest($"{_url}/{path}", UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-memorypack");

            // request.
            yield return request.SendWebRequest();

            Debug.Log($"msg: {request.result}, error: {request.error}, code: {request.responseCode}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 응답 가져오기.
                byte[] responseData = request.downloadHandler.data;
                var resData = MemoryPackDataSerializer.Deserialize<RES_CreatePlayer>(responseData);

                _playerDataManager.SetPlayer(userId, uid, resData.Pid, resData.PlayerName);

                StartCoroutine(WebReq_Login(_playerDataManager.UserId, _playerDataManager.Uid, resData.Pid, _playerDataManager.AccessToken));
            }
            else
            {
                _noticeText.text = "Failed to create player";

                yield return new WaitForSeconds(5f);

                _noticeText.text = string.Empty;
            }
        }
    }


    /// <summary>
    /// Request create player.
    /// </summary>
    public IEnumerator WebReq_Login(string userId, string uid, string pid, string accessToken)
    {
        const string path = "auth/login";

        var postData = MemoryPackDataSerializer.Serialize(new REQ_Login
        {
            UserId = userId,
            Uid = uid,
            Pid = pid,
            AccessToken = accessToken,
        });

        using (var request = new UnityWebRequest($"{_url}/{path}", UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-memorypack");

            // request.
            yield return request.SendWebRequest();

            Debug.Log($"msg: {request.result}, error: {request.error}, code: {request.responseCode}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 응답 데이터 받아오기
                byte[] responseData = request.downloadHandler.data;
                // 응답 데이터를 MemoryPack을 사용하여 역직렬화
                var resData = MemoryPackDataSerializer.Deserialize<RES_Login>(responseData);

                _playerDataManager.SetPlayerInfo(resData.PlayerInfo);

                _noticeText.text = "Success to log in";

                yield return new WaitForSeconds(5f);

                // Load scene.
                SceneManager.LoadScene(_loadScene);
            }
            else
            {
                _noticeText.text = "Failed to log in";

                yield return new WaitForSeconds(5f);

                _noticeText.text = string.Empty;
            }
        }
    }
}
