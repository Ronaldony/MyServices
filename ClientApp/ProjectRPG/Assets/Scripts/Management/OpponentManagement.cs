
using GameCore;
using GameCore.Datas.Play;
using GameCore.GamePacket;
using GamePackets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class OpponentManagement : Singleton<OpponentManagement>
{
    [SerializeField] private GameObject _opponentDummyPrefab;
    [SerializeField] private GameObject _opponentPrefab;

    private SectionManagement _sectionManagement;

    // Queue.
    private ConcurrentQueue<UpdateGamePacket> _updatePackets;
    private ConcurrentQueue<long> _destroyOpponents;

    /// <summary>
    /// Opponent dictionary.
    /// key: Mob unit id.
    /// value: Enemy GameObject.
    /// </summary>
    private Dictionary<long, GameObject> _opponentDict;

    public OpponentManagement()
    {
        _updatePackets = new ConcurrentQueue<UpdateGamePacket>();
        _destroyOpponents = new ConcurrentQueue<long>();
        _opponentDict = new Dictionary<long, GameObject>();
    }

    private void Start()
    {
        _sectionManagement = SectionManagement.Instance;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!GameLoadManager.Instance.IsLoadComplete())
        {
            return;
        }

        // Process alive enemy.
        Proc_DestroyOpponents();

        // Process update packet.
        Proc_UpdatePacket();

        // Process section.
        Proc_Section();
    }

    /// <summary>
    /// Enqueue update packet.
    /// </summary>
    public void AddUpdatePacket(UpdateGamePacket updatePacket)
    {
        _updatePackets.Enqueue(updatePacket);
    }

    private void Proc_UpdatePacket()
    {
        // Process update pakcet.
        while (_updatePackets.TryDequeue(out var updatePacket))
        {
            switch (updatePacket.PacketCode)
            {
                case GamePacketCode.SC_EnterPlayer:
                    {
                        var gp = updatePacket.Data as GamePacketSC_EnterPlayer;
                        SpawnOpponents(gp.PlayInfos);
                    }
                    break;
                case GamePacketCode.SC_SpawnUnits:
                    {
                        var gp = updatePacket.Data as GamePacketSC_SpawnUnits;
                        SpawnOpponents(gp.PlayInfos);
                    }
                    break;
                case GamePacketCode.SC_UpdateUnitStat:
                    {
                        var gp = updatePacket.Data as GamePacketSC_UpdateUnitStat;
                        UpdateStat(gp);
                    }
                    break;
                case GamePacketCode.SC_UpdateSection:
                    {
                        var gp = updatePacket.Data as GamePacketSC_UpdateSection;
                        SpawnOpponents(new Dictionary<long, DTO_PlayInfo>(gp.PlayInfos));
                    }
                    break;
                case GamePacketCode.SC_UpdateSectionNewEnter:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateSectionNewEnter;
                        SpawnOpponents(packet.PlayInfos);
                    }
                    break;
                case GamePacketCode.SC_MoveStart:
                    {
                        var gp = updatePacket.Data as GamePacketSC_MoveStart;
                        MoveStart(gp.UnitId, gp);
                    }
                    break;
                case GamePacketCode.SC_MoveEnd:
                    {
                        var gp = updatePacket.Data as GamePacketSC_MoveEnd;
                        MoveEnd(gp.UnitId, gp);
                    }
                    break;
                case GamePacketCode.SC_MoveFixed:
                    {
                        var gp = updatePacket.Data as GamePacketSC_MoveFixed;
                        MoveFixed(gp.UnitId, gp.FixedPos.X, gp.FixedPos.Y);
                    }
                    break;
                case GamePacketCode.SC_Dash:
                    {
                        var gp = updatePacket.Data as GamePacketSC_Dash;
                        Dash(gp.UnitId, gp.DashEnd.X, gp.DashEnd.Y);
                    }
                    break;
                case GamePacketCode.SC_KnockBack:
                    {
                        var gp = updatePacket.Data as GamePacketSC_KnockBack;
                        Knockback(gp.UnitId, gp);
                    }
                    break;
                case GamePacketCode.SC_AttackMelee:
                    {
                        var gp = updatePacket.Data as GamePacketSC_AttackMelee;
                        AttackMelee(gp.UnitId, gp);
                    }
                    break;
                case GamePacketCode.SC_AttackProjectile:
                    {
                        var gp = updatePacket.Data as GamePacketSC_AttackProjectile;
                        AttackProjectile(gp.UnitId, gp);
                    }
                    break;
                case GamePacketCode.SC_SwitchWeapon:
                    {
                        var gp = updatePacket.Data as GamePacketSC_SwitchWeapon;
                        SwitchWeapon(gp.UnitId, gp.WeaponId);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// SpawnOpponents.
    /// </summary>
    private void SpawnOpponents(Dictionary<long, DTO_PlayInfo> playInfos)
    {
        // Remove player unit id.
        playInfos.Remove(PlayerDataManager.Instance.UnitId);

        foreach (var unit in playInfos)
        {
            var unitId = unit.Key;
            var playInfo = unit.Value;

            // Opponent.
            var opponent = FindOpponent(unitId);

            if (null != opponent)
            {
                // 이미 존재하는 유닛.
                var opponentController = opponent.GetComponent<OpponentController>();
                opponentController.Setup(playInfo.Location);
                continue;
            }

            if (false == SpawnOpponent(unitId, playInfo))
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Spawn opponent.
    /// </summary>
    private bool SpawnOpponent(long unitId, DTO_PlayInfo playInfo)
    {
        // PlayInfos.
        var location = playInfo.Location;
        var stat = playInfo.Stat;

        // Opponent 생성.
        var opponentDummyObj = Instantiate(_opponentDummyPrefab, new Vector3(location.Pos.X, location.Pos.Y), Quaternion.identity);
        var opponent = Instantiate(_opponentPrefab, new Vector3(location.Pos.X, location.Pos.Y), Quaternion.identity);

        // Add opponent.
        if (false == AddOpponent(unitId, opponent))
        {
            Debug.Log($"Spawn opponent fail.");
            Destroy(opponent);
            return false;
        }

        // Controller.
        var controller = opponent.GetComponent<OpponentController>();
        var dummy = opponentDummyObj.GetComponent<UnitDummy>();

        // Setups
        controller.Setup(playInfo.Location, dummy);

        // Health.
        var health = opponent.GetComponent<OpponentHealth>();
        health.Setup(stat.Health, stat.HealthMax);

        // Weapon.
        var weaponInventory = opponent.GetComponent<OpponentWeaponInventory>();
        weaponInventory.ChangeActiveWeapon(playInfo.Equipment.Weapon);

        // NameTag.
        var nameTag = opponent.GetComponentInChildren<NameTag>();
        nameTag.SetPlayerName(playInfo.PlayerName);

        return true;
    }

    /// <summary>
    /// Update movement.
    /// </summary>
    private void SwitchWeapon(long unitId, int weaponId)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Controller.
        var weaponInventory = opponent.GetComponent<OpponentWeaponInventory>();
        weaponInventory.ChangeActiveWeapon(weaponId);
    }

    /// <summary>
    /// Move start.
    /// </summary>
    private void MoveStart(long unitId, GamePacketSC_MoveStart packet)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Controller.
        var controller = opponent.GetComponent<OpponentController>();
        controller.AddMsg(new OpponentMsg_MoveStart 
        {
            MoveDir = packet.Direction
        });
    }

    /// <summary>
    /// Move end.
    /// </summary>
    private void MoveEnd(long unitId, GamePacketSC_MoveEnd packet)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Controller.
        var controller = opponent.GetComponent<OpponentController>();
        controller.AddMsg(new OpponentMsg_MoveEnd
        {
            EndPos = packet.EndPos
        });
    }

    /// <summary>
    /// Update stat.
    /// </summary>
    private void UpdateStat(GamePacketSC_UpdateUnitStat packet)
    {
        foreach (var pair in packet.UnitStats)
        {
            // Opponent.
            var opponent = FindOpponent(pair.Key);

            if (null == opponent)
            {
                continue;
            }

            // Stat.
            var health = opponent.GetComponent<OpponentHealth>();
            health.UpdateHealth(pair.Value.Health);
        }
    }

    /// <summary>
    /// Attack melee.
    /// </summary>
    private void AttackMelee(long unitId, GamePacketSC_AttackMelee packet)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Attack melee.
        var weaponInventory = opponent.GetComponent<OpponentWeaponInventory>();
        weaponInventory.AttackMelee(packet.Angle, packet.Rotation);
    }

    /// <summary>
    /// Attack projectile.
    /// </summary>
    private void AttackProjectile(long unitId, GamePacketSC_AttackProjectile packet)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Attack melee.
        var weaponInventory = opponent.GetComponent<OpponentWeaponInventory>();
        weaponInventory.AttackProjectile(packet.Projectile.SpawnPos, packet.Projectile.TargetPos);
    }

    /// <summary>
    /// Move fixed.
    /// </summary>s
    private void MoveFixed(long unitId, float x, float y)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Move fixed.
        var controller = opponent.GetComponent<OpponentController>();

        controller.AddMsg(new OpponentMsg_MoveFix
        {
            FixPos = new Vector2(x, y)
        });
    }

    /// <summary>
    /// Dash.
    /// </summary>
    private void Dash(long unitId, float x, float y)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Dash.
        var controller = opponent.GetComponent<OpponentController>();
        //controller.Dash(new Vector2(x, y));
        controller.AddMsg(new OpponentMsg_Dash
        {
            DashedPos = new Vector2(x, y)
        });
    }

    /// <summary>
    /// Knockback.
    /// </summary>
    private void Knockback(long unitId, GamePacketSC_KnockBack packet)
    {
        // Opponent.
        var opponent = FindOpponent(unitId);

        if (null == opponent)
        {
            return;
        }

        // Knockback.
        var controller = opponent.GetComponent<OpponentController>();
        //controller.Knockback(packet.SrcPosition, packet.Position);
        controller.AddMsg(new OpponentMsg_Knockback
        {
            Position = packet.Position,
            SrcPosition = packet.SrcPosition
        });
    }

    /// <summary>
    /// Find opponent.
    /// </summary>
    private GameObject FindOpponent(long unitId)
    {
        if (_opponentDict.TryGetValue(unitId, out var opponent))
        {
            return opponent;
        }

        return null;
    }

    /// <summary>
    /// Find opponent.
    /// </summary>
    private bool AddOpponent(long unitId, GameObject opponent)
    {
        return _opponentDict.TryAdd(unitId, opponent);
    }

    /// <summary>
    /// Process section.
    /// </summary>
    private void Proc_Section()
    {
        var removemobs = new List<long>();
        var playerPos = PlayerController.Instance.GetPosition();
        var playerSection = GetSection((int)playerPos.x, (int)playerPos.y);

        // Scan enemy.
        foreach (var pair in _opponentDict)
        {
            var controller = pair.Value.GetComponent<OpponentController>();
            if (null == controller)
            {
                continue;
            }

            try
            {
                var position = controller.GetPosition();
                var section = GetSection((int)position.x, (int)position.y);

                // Check around section.
                if (false == _sectionManagement.IsAroundSection(playerSection, section))
                {
                    removemobs.Add(pair.Key);
                    Debug.Log($"Destroy mob. player section: {playerSection}, mob section: {section}");
                }
            }
            catch(Exception ex)
            {
                //Debug.Log(ex.Message);
                continue;
            }
        }

        // Remove enemies.
        foreach (var unitId in removemobs)
        {
            // Remove enemy.
            _opponentDict.Remove(unitId, out var opponent);

            var opponentController = opponent.GetComponent<OpponentController>();
            var opponentDummy = opponentController.GetDummy();

            // Destroy enemy.
            Debug.Log($"Proc_Section - Destroy opponent.");
            Destroy(opponentDummy.gameObject);
            Destroy(opponent);
        }
    }

    /// <summary>
    /// Procewss destroy enemy.
    /// </summary>
    private void Proc_DestroyOpponents()
    {
        while (_destroyOpponents.TryDequeue(out var unitId))
        {
            var opponent = _opponentDict[unitId];

            var opponentController = opponent.GetComponent<OpponentController>();
            var opponentDummy = opponentController.GetDummy();

            // Destroy enemy.
            Debug.Log($"Destroy opponent.");
            Destroy(opponentDummy.gameObject);
            Destroy(opponent);

            // Remove enemy.
            _opponentDict.Remove(unitId);
        }
    }

    /// <summary>
    /// Kill opponent.
    /// </summary>
    public void DestroyOpponent(long unitId)
    {
        _destroyOpponents.Enqueue(unitId);
    }

    /// <summary>
    /// Get section or default.
    /// </summary>
    private (int sectionX, int sectionY) GetSection(int posX, int posY)
    {
        return (posX / GameData.SECTION_WIDTH, posY / GameData.SECTION_HEIGHT);
    }
}