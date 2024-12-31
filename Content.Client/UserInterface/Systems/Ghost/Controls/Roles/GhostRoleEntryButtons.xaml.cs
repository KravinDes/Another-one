﻿using Content.Shared.Ghost.Roles;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles;

[GenerateTypedNameReferences]
public sealed partial class GhostRoleEntryButtons : BoxContainer
{
    [Dependency] private readonly IGameTiming _timing = default!;
    private readonly GhostRoleKind _ghostRoleKind;
    private readonly uint _playerCount;
    private readonly TimeSpan _raffleEndTime = TimeSpan.MinValue;

    public GhostRoleEntryButtons(GhostRoleInfo ghostRoleInfo)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _ghostRoleKind = ghostRoleInfo.Kind;
        if (IsActiveRaffle(_ghostRoleKind))
        {
            _playerCount = ghostRoleInfo.RafflePlayerCount;
            _raffleEndTime = ghostRoleInfo.RaffleEndTime;
        }

        UpdateRequestButton();
    }

    private void UpdateRequestButton()
    {
        var messageId = _ghostRoleKind switch
        {
            GhostRoleKind.FirstComeFirstServe => "ghost-roles-window-request-role-button",
            GhostRoleKind.RaffleReady => "ghost-roles-window-join-raffle-button",
            GhostRoleKind.RaffleInProgress => "ghost-roles-window-raffle-in-progress-button",
            GhostRoleKind.RaffleJoined => "ghost-roles-window-leave-raffle-button",
            _ => throw new ArgumentOutOfRangeException(nameof(_ghostRoleKind),
                $"Unknown {nameof(GhostRoleKind)} '{_ghostRoleKind}'")
        };

        if (IsActiveRaffle(_ghostRoleKind))
        {
            var timeLeft = _timing.CurTime <= _raffleEndTime
                ? _raffleEndTime - _timing.CurTime
                : TimeSpan.Zero;

            var timeString = $"{timeLeft.Minutes:0}:{timeLeft.Seconds:00}";
            RequestButton.Text = Loc.GetString(messageId, ("time", timeString), ("players", _playerCount));
        }
        else
        {
            RequestButton.Text = Loc.GetString(messageId);
        }
    }

    private static bool IsActiveRaffle(GhostRoleKind kind)
    {
        return kind is GhostRoleKind.RaffleInProgress or GhostRoleKind.RaffleJoined;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        if (IsActiveRaffle(_ghostRoleKind))
        {
            UpdateRequestButton();
        }
    }
}