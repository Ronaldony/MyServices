using GameCore;
using GameCore.Datas.Post;
using GameCore.HubPacket;
using HubPackets;
using UnityEngine;
using UnityEngine.UI;

public class Post : MonoBehaviour
{
    private DTO_Post _post;

    /// <summary>
    /// Set slot.
    /// </summary>
    public void SetPost(DTO_Post post)
    {
        _post = post;

        var text = GetComponentInChildren<Text>();
        text.text = _post.Text;

        // Already claimed.
        if (post.IsClaimed)
        {
            DisablePostBG();
        }
    }

    /// <summary>
    /// On claim for button.
    /// </summary>
    public void OnClaim()
    {
        // 이미 수령하였는지 확인.
        if (_post.IsClaimed)
        {
            Debug.LogError($"post already claimed. post uid: {_post.PostUid}");
            return;
        }

        // Send packet.
        HubPacketManager.Instance.Send<HubPacketCS_ClaimPost>(HubPacketCode.CS_ClaimPost, new HubPacketCS_ClaimPost
        {
            PostUid = _post.PostUid
        });
    }

    /// <summary>
    /// Claim post.
    /// </summary>
    public void Claim()
    {
        // Claim items.
        _post.IsClaimed = true;

        var inventory = Inventory.Instance;

        foreach (var item in _post.Items)
        {
            inventory.PickupItem(item);
        }

        // Disable background.
        DisablePostBG();

        // Inactive buttons.
        foreach (var button in GetComponentsInChildren<Button>())
        {
            button.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// On delete for button.
    /// </summary>
    public void OnDelete()
    {
        // 이미 수령하였는지 확인.
        if (false == _post.IsClaimed)
        {
            Debug.LogError($"post not claimed. post uid: {_post.PostUid}");
            return;
        }

        // Send packet.
        HubPacketManager.Instance.Send<HubPacketCS_DeletePost>(HubPacketCode.CS_DeletePost, new HubPacketCS_DeletePost
        {
            PostUid = _post.PostUid
        });
    }

    private void DisablePostBG()
    {
        // Change background.
        foreach (var image in GetComponentsInChildren<Image>())
        {
            if (image.name == "NormalBG")
            {
                image.gameObject.SetActive(false);
            }
            else if (image.name == "DisableBG")
            {
                image.gameObject.SetActive(true);
            }
        }
    }
}
