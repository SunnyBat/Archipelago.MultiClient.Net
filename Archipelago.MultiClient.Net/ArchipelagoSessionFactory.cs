﻿using System;
using Archipelago.MultiClient.Net.Cache;
using Archipelago.MultiClient.Net.Helpers;

namespace Archipelago.MultiClient.Net
{
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
            var socket = new ArchipelagoSocketHelper(uri.ToString());
            var dataPackageCache = new DataPackageFileSystemCache(socket);
            var items = new ReceivedItemsHelper(socket, dataPackageCache);
            var locations = new LocationCheckHelper(socket);
            return new ArchipelagoSession(socket, items, locations, dataPackageCache);
        }

        /// <summary>
        ///     Creates an <see cref="ArchipelagoSession"/> object which facilitates all communication to the Archipelago server.            
        /// </summary>
        /// <param name="hostname">
        ///     The hostname of the Archipelago server. Ex: `archipelago.gg` or `localhost`
        /// </param>
        /// <param name="port">
        ///     The port number which the Archipelago server is hosted on. Defaults to: 38281
        /// </param>
        public static ArchipelagoSession CreateSession(string hostname, int port = 38281)
        {
            var uriBuilder = new UriBuilder("ws", hostname, port);
            return CreateSession(uriBuilder.Uri);
        }
    }
}
