﻿using Archipelago.MultiClient.Net.Cache;
using Archipelago.MultiClient.Net.Helpers;
using System;

namespace Archipelago.MultiClient.Net
{
	/// <summary>
	/// Factory to initiate a new ArchipelagoSession, the base object to communicate with an Archipelago Server
	/// </summary>
	public static class ArchipelagoSessionFactory
    {
        /// <summary>
        ///     Creates an <see cref="ArchipelagoSession"/> object which facilitates all communication to the Archipelago server.
        /// </summary>
        /// <param name="uri">
        ///     The full URI to the Archipelago server, including scheme, hostname, and port.
        /// </param>
        public static ArchipelagoSession CreateSession(Uri uri)
        {
            var socket = new ArchipelagoSocketHelper(uri);
            var dataPackageCache = new DataPackageCache(socket);
            var locations = new LocationCheckHelper(socket, dataPackageCache);
            var items = new ReceivedItemsHelper(socket, locations, dataPackageCache);
            var players = new PlayerHelper(socket);
            var roomState = new RoomStateHelper(socket, locations);
            var connectionInfo = new ConnectionInfoHelper(socket);
            var dataStorage = new DataStorageHelper(socket, connectionInfo);
            var messageLog = new MessageLogHelper(socket, items, locations, players, connectionInfo);

            return new ArchipelagoSession(socket, items, locations, players, roomState, connectionInfo, dataStorage, messageLog);
        }

		/// <summary>
		///     Creates an <see cref="ArchipelagoSession"/> object which facilitates all communication to the Archipelago server.            
		/// </summary>
		/// <param name="hostname">
		///     The hostname of the Archipelago server, can include protocol and port.
		///			Ex: `archipelago.gg`, `localhost`, `localhost:38281` or `ws://archipelago.gg:46376`
		/// </param>
		/// <param name="port">
		///     (Optional) The port number which the Archipelago server is hosted on. Defaults to: 38281,
		///			will be ignored if the port is added to the hostname
		/// </param>
		public static ArchipelagoSession CreateSession(string hostname, int port = 38281) => CreateSession(ParseUri(hostname, port));

        internal static Uri ParseUri(string hostname, int port)
        {
	        var uri = hostname;

			if (!uri.StartsWith("ws://") && !uri.StartsWith("wss://"))
		        uri = "unspecified://" + uri;
			if (!uri.Substring(uri.IndexOf("://", StringComparison.Ordinal) + 3).Contains(":"))
		        uri += $":{port}";
	        if (uri.EndsWith(":"))
		        uri += port;
			
	        return new Uri(uri);
        }
    }
}
