﻿// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetClient
 *   Bandled GUI client for MetaTweet
 *   Part of MetaTweet
 * Copyright c 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of MetaTweetClient.
 * 
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at your
 * option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
 * License for more details. 
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>,
 * or write to the Free Software Foundation, Inc., 51 Franklin Street,
 * Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Threading;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Collections.Generic;
using XSpect.MetaTweet.ObjectModel;
using System.Linq;
using XSpect.MetaTweet.Modules;

namespace XSpect.MetaTweet.Clients
{
    public class MetaTweetClient
        : Object
    {
        private readonly TcpClientChannel _channel = new TcpClientChannel();
        
        private ServerCore _host;

        public ServerCore Host
        {
            get
            {
                return this._host;
            }
        }

        public void Connect()
        {
            ChannelServices.RegisterChannel(this._channel, false);
            RemotingConfiguration.RegisterWellKnownClientType(typeof(ServerCore), "tcp://localhost:7784/core");
            this._host = new ServerCore();
        }

        public void Disconnect()
        {
            this._host = null;
            ChannelServices.UnregisterChannel(this._channel);
        }
    }
}