﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.HabboRoleplay.Houses;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class KickUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (Session.GetHabbo().Rank == 2 && Session.GetHabbo().GetPermissions().HasRight("ambassador"))
            {
                Session.SendNotification("Esta ação do Embaixador está atualmente desativada.");
                return;
            }

            if (!Room.CheckRights(Session) && Room.WhoCanKick != 2 && Room.Group == null)
                return;

            if (Room.Group != null && !Room.CheckRights(Session, false, true))
                return;

            House House;
            if (!Room.TryGetHouse(out House))
            {
                Session.SendWhisper("Você não está dentro de um apartamento!", 1);
                return;
            }

            if (House.OwnerId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Você não é o proprietário do apartamento!", 1);
                return;
            }

            int UserId = Packet.PopInt();
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
            if (User == null || User.IsBot)
                return;

            //Cannot kick owner or moderators.
            if (Room.CheckRights(User.GetClient(), true) || User.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            Room.GetRoomUserManager().RemoveUserFromRoom(User.GetClient(), true, true);
            //PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModKickSeen", 1);
        }
    }
}
