﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RdtClient.Service.Services
{
    public interface IRemoteService
    {
        Task Update();
    }

    public class RemoteService : IRemoteService
    {
        private readonly IHubContext<RdtHub> _hub;
        private readonly ITorrents _torrents;

        public RemoteService(IHubContext<RdtHub> hub, ITorrents torrents)
        {
            _hub = hub;
            _torrents = torrents;
        }

        public async Task Update()
        {
            var torrents = await _torrents.Get();
            
            // Prevent infinite recursion when serializing
            foreach (var file in torrents.SelectMany(torrent => torrent.Downloads))
            {
                file.Torrent = null;
            }
            
            await _hub.Clients.All.SendCoreAsync("update",
                                                 new Object[]
                                                 {
                                                     torrents
                                                 });
        }
    }
}
