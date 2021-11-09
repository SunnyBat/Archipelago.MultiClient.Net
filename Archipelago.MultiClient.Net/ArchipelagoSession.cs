﻿using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Cache;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;

namespace Archipelago.MultiClient.Net
{
    public class ArchipelagoSession
    {
        public ArchipelagoSocketHelper Socket { get; }

        public ReceivedItemsHelper Items { get; }

        public LocationCheckHelper Locations { get; }

        private DataPackageFileSystemCache DataPackageCache { get; }
        
        private bool expectingDataPackage = false;
        private Action<DataPackage> dataPackageCallback;

        internal ArchipelagoSession(ArchipelagoSocketHelper socket,
                                    ReceivedItemsHelper items,
                                    LocationCheckHelper locations)
        {
            Socket = socket;
            Items = items;
            Locations = locations;
            DataPackageCache = new DataPackageFileSystemCache(socket);

            socket.PacketReceived += Socket_PacketReceived;
        }

        private void Socket_PacketReceived(ArchipelagoPacketBase packet)
        {
            switch (packet.PacketType)
            {
                case ArchipelagoPacketType.DataPackage:
                    {
                        if (expectingDataPackage)
                        {
                            var dataPackagePacket = (DataPackagePacket)packet;

                            DataPackageCache.SaveDataPackageToCache(dataPackagePacket.DataPackage);

                            if (dataPackageCallback != null)
                            {
                                dataPackageCallback(dataPackagePacket.DataPackage);
                            }

                            expectingDataPackage = false;
                            dataPackageCallback = null;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        ///     Attempts to retrieve the datapackage from cache or from the server, if there is no cached version.
        ///     Calls a callback method when retrieval is successful.
        /// </summary>
        /// <param name="callback">
        ///     Action to call when the datapackage is received or retrieved from cache.
        /// </param>
        public void GetDataPackageAsync(Action<DataPackage> callback)
        {
            if (DataPackageCache.TryGetDataPackageFromCache(out var package))
            {
                if (callback != null)
                {
                    callback(package);
                }
            }
            else
            {
                Socket.SendPacket(new GetDataPackagePacket());
                expectingDataPackage = true;
                dataPackageCallback = callback;
            }
        }

        /// <summary>
        ///     Attempt to log in to the Archipelago server by opening a websocket connection and sending a Connect packet.
        /// </summary>
        /// <param name="game">The game this client is playing.</param>
        /// <param name="name">The slot name of this client.</param>
        /// <param name="version">The minimum AP protocol version this client supports.</param>
        /// <param name="tags">The tags this client supports.</param>
        /// <param name="uuid">The uuid of this client.</param>
        /// <param name="password">The password to connect to this AP room.</param>
        public void AttemptConnectAndLogin(string game, string name, Version version, List<string> tags = null, string uuid = null, string password = null)
        {
            if (uuid == null)
            {
                uuid = Guid.NewGuid().ToString();
            }

            if (tags == null)
            {
                tags = new List<string>();
            }

            Socket.Connect();
            Socket.SendPacket(new ConnectPacket()
            {
                Game = game,
                Name = name,
                Password = password,
                Tags = tags,
                Uuid = uuid,
                Version = version
            });
        }
    }
}