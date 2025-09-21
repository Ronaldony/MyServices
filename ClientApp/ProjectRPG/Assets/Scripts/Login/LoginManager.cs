using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using GameCore.Http;
using GameCore.Http.Auth;
using GameCore.HubPacket;
using GamePackets;
using HubPackets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{

    [SerializeField] private TMP_Text _noticeText;
	[SerializeField] private string _url;
    [SerializeField] private string _loadScene;

    private PlayerDataManager _playerDataManager;
    private GamePacketManager _gamePacketManager;
    private HubPacketManager _hubPacketManager;

    /// <summary>
    /// Web headers.
    /// key: header key.
    /// value: header value.
    /// </summary>
    private Dictionary<string, string> _authHeaders;

    private void Start()
    {
        _playerDataManager = PlayerDataManager.Instance;
        _gamePacketManager = GamePacketManager.Instance;
        _hubPacketManager = HubPacketManager.Instance;

        _authHeaders = new Dictionary<string, string>();
    }

    /// <summary>
    /// Request auth.
    /// </summary>
	public IEnumerator Request_Auth(string userId)
	{
        _noticeText.text = "Requesting log in";

        // Packet.F
        var httpRouteAttr = typeof(REQ_Auth).GetAttribute(typeof(HttpRouteAttribute)) as HttpRouteAttribute;
		var body = MemoryPackDataSerializer.Serialize(new REQ_Auth
        {
            UserId = userId
        });

        // Request web.
        yield return WebRequestPost($"{_url}/{httpRouteAttr.Path}", body, (request) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 응답 가져오기.
                byte[] responseData = request.downloadHandler.data;
                var res = MemoryPackDataSerializer.Deserialize<RES_Auth>(responseData);

                // Set access token.
                _playerDataManager.SetAccessToken(res.AccessToken, res.RefreshToken, res.RefrshTokenExpire);

                // Reset web headers.
                _authHeaders.Clear();
                _authHeaders.Add("uid", res.Uid);
                _authHeaders.Add("accesstoken", res.AccessToken);

                // Process player.
                if (false == res.IsNewPlayer)
                {
                    // 기존 플레이어.
                    StartCoroutine(Request_Login(userId, res.Uid, res.Pid, res.AccessToken));
                }
                else
                {
                    // 새로운 플레이어
                    StartCoroutine(Request_CreatePlayer(userId, res.Uid, res.AccessToken));
                }
            }
            else
            {
                _noticeText.text = "Failed to authentice";
            }
        });
    }

    /// <summary>
    /// Request create player.
    /// </summary>
    public IEnumerator Request_CreatePlayer(string userId, string uid, string accessToken)
    {
        // Packet.
        var httpRouteAttr = typeof(REQ_CreatePlayer).GetAttribute(typeof(HttpRouteAttribute)) as HttpRouteAttribute;
        var body = MemoryPackDataSerializer.Serialize(new REQ_CreatePlayer
        {
            UserId = userId,
        });

        // Request web.
        yield return WebRequestPost($"{_url}/{httpRouteAttr.Path}", body, (request) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 응답 가져오기.
                byte[] responseData = request.downloadHandler.data;
                var res = MemoryPackDataSerializer.Deserialize<RES_CreatePlayer>(responseData);

                // Set Player.
                _playerDataManager.SetPlayer(userId, uid, res.Pid, res.PlayerName);

                // Login.
                StartCoroutine(Request_Login(userId, uid, res.Pid, accessToken));

            }
            else
            {
                _noticeText.text = "Failed to create player";
            }
        });
    }

    /// <summary>
    /// Request create player.
    /// </summary>
    public IEnumerator Request_Login(string userId, string uid, string pid, string accessToken)
    {
        // Packet.
        var httpRouteAttr = typeof(REQ_Login).GetAttribute(typeof(HttpRouteAttribute)) as HttpRouteAttribute;
        var body = MemoryPackDataSerializer.Serialize(new REQ_Login
        {
            UserId = userId,
        });

        // Request web.
        yield return WebRequestPost($"{_url}/{httpRouteAttr.Path}", body, (request) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 응답 가져오기.
                byte[] responseData = request.downloadHandler.data;
                var res = MemoryPackDataSerializer.Deserialize<RES_Login>(responseData);

                _playerDataManager.PlayerInfo = res.PlayerInfo;
                _playerDataManager.Pid = pid;

                // Net packet login.
                var connectToGame = _gamePacketManager.Connect();
                if (connectToGame == false)
                {
                    _noticeText.text = "Fail to connect to game server";
                    return;
                }

                var connectToHub = _hubPacketManager.Connect();
                if (connectToHub == false)
                {
                    _gamePacketManager.Disconnect();
                    _noticeText.text = "Fail to connect to hub server";
                    return;
                }

                // Log in world server.
                StartCoroutine(LoginWorldServer(pid, accessToken));
            }
            else
            {
                _noticeText.text = "Failed to log in";
            }
        });
    }

    /// <summary>
    /// Log in world server.
    /// </summary>
    private IEnumerator LoginWorldServer(string pid, string token)
    {
        // Delay 2 seconds.
        yield return new WaitForSeconds(2f);

        // Send Login packet.
        var hubPacket = new HubPacketCS_Enter
        {
            Pid = pid,
            Token = token,
        };

        // Send packet.
        if (false == _hubPacketManager.Send<HubPacketCS_Enter>(HubPacketCode.CS_Enter, hubPacket))
        {
            _noticeText.text = "Fail to send pakcet login.";
        }
        else
        {
            _noticeText.text = "Success to log in. Loading scene...";
        }
    }

    /// <summary>
    /// Web request - post.
    /// </summary>
    private IEnumerator WebRequestPost(string url, byte[] body, Action<UnityWebRequest> action)
    {
        using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-memorypack");

            // Set headers.
            foreach (var header in _authHeaders)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            yield return request.SendWebRequest();

            action(request);
        }
    }
}
