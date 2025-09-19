using GameCore;
using GameCore.Datas.Post;
using GameCore.HubPacket;
using HubPackets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class PostBox : Singleton<PostBox>
{
    [SerializeField] private GameObject _postPrefab;
    [SerializeField] private GameObject _postBoxObj;
    [SerializeField] private GameObject _slotObj;
    
    /// <summary>
    /// Posts.
    /// key: Post uid.
    /// value: Post
    /// </summary>
    private readonly Dictionary<string, Post> _posts;

    private bool _showPostBox = false;

    private readonly ConcurrentQueue<UpdateHubPacket> _hubPackets;

    public PostBox()
    {
        _posts = new Dictionary<string, Post>();
        _hubPackets = new ConcurrentQueue<UpdateHubPacket>();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        if (_hubPackets.TryDequeue(out var updatePacket))
        {
            switch (updatePacket.PacketCode)
            {
                case HubPacketCode.SC_Enter:
                    {
                        var packet = updatePacket.Data as HubPacketSC_Enter;
                        foreach (var post in packet.PostInfo.Posts)
                        {
                            AddPost(post.Value);
                        }
                    }
                    break;
                case HubPacketCode.SC_PushPost:
                    {
                        var packet = updatePacket.Data as HubPacketSC_PushPost;
                        foreach (var post in packet.PushPosts)
                        {
                            AddPost(post);
                        }
                    }
                    break;
                case HubPacketCode.SC_DeletePost:
                    {
                        var packet = updatePacket.Data as HubPacketSC_DeletePost;
                        if (packet.IsDeleted)
                        {
                            DeletePost(packet.PostUid);
                        }
                    }
                    break;
                case HubPacketCode.SC_ClaimPostFail:
                    {
                        var packet = updatePacket.Data as HubPacketSC_ClaimPostFail;
                        Debug.LogError($"Fail claim post");
                    }
                    break;
                case GamePacketCode.SC_ClaimPost:
                    {
                        var packet = updatePacket.Data as GamePacketSC_ClaimPost;
                        ClaimPost(packet.PostUid);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Add hub pakcet.
    /// </summary>
    public void AddUpdatePacket(UpdateHubPacket packet)
    {
        _hubPackets.Enqueue(packet);
    }

    /// <summary>
    /// Toggle postBox.
    /// </summary>
    public void Toggle()
    {
        Debug.Log($"TogglePostBox");

        if (_showPostBox)
        {
            // Off inventory.
            _showPostBox = false;
            _postBoxObj.SetActive(false);
        }
        else
        {
            // On inventory.
            _showPostBox = true;
            _postBoxObj.SetActive(true);
        }
    }

    /// <summary>
    /// Is show post box.
    /// </summary>
    public bool IsShowPostBox()
    {
        return _showPostBox;
    }

    /// <summary>
    /// Add post.
    /// </summary>
    public void AddPost(DTO_Post dtoPost)
    {
        var postPrefab = Instantiate(_postPrefab, transform.position, Quaternion.identity, _slotObj.transform);
        var post = postPrefab.GetComponent<Post>();

        if (post == null)
        {
            Debug.LogError($"get post error.");
            return;
        }

        _posts.Add(dtoPost.PostUid, post);

        // Set post.
        post.SetPost(dtoPost);
    }

    /// <summary>
    /// Claim post.
    /// </summary>
    private void ClaimPost(string postUid)
    {
        // Post uid 검사.
        if (false == _posts.ContainsKey(postUid))
        {
            Debug.LogError($"not exist post. post uid: {postUid}");
            return;
        }

        // 이미 수령하였는지 확인.
        var post = _posts[postUid];
        post.Claim();
    }

    /// <summary>
    /// Delete post.
    /// </summary>
    private void DeletePost(string postUid)
    {
        // Post uid 검사.
        if (false == _posts.ContainsKey(postUid))
        {
            Debug.LogError($"not exist post. post uid: {postUid}");
            return;
        }

        // 이미 수령하였는지 확인.
        var post = _posts[postUid];

        // Destroy post.
        Destroy(post.gameObject);
    }

    /// <summary>
    /// Fail claim process.
    /// </summary>
    private void FailClaim(string postUid)
    {
        // Post uid 검사.
        if (false == _posts.ContainsKey(postUid))
        {
            Debug.LogError($"not exist post. post uid: {postUid}");
            return;
        }

        // 이미 수령하였는지 확인.
        var post = _posts[postUid];
        post.Claim();
    }
}
