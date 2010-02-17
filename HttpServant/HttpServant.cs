﻿// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * HttpServant
 *   MetaTweet Servant which provides HTTP service
 *   Part of MetaTweet
 * Copyright © 2008-2010 Takeshi KIRIYA (aka takeshik) <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of HttpServant.
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using Achiral;
using Achiral.Extension;
using XSpect.Extension;
using XSpect.MetaTweet.Modules;
using System.Timers;
using XSpect.MetaTweet.Modules.Properties;
using ServerResources = XSpect.MetaTweet.Properties.Resources;

namespace XSpect.MetaTweet.Modules
{
    public class HttpServant
        : ServantModule
    {
        private static readonly ImageConverter _imageConverter = new ImageConverter();

        private readonly HttpListener _listener;

        public HttpServant()
        {
            this._listener = new HttpListener();
        }

        protected override void InitializeImpl()
        {
            this._listener.Prefixes.AddRange(this.Configuration.ResolveValue<List<String>>("prefixes"));
            base.InitializeImpl();
        }

        protected override void StartImpl()
        {
            this._listener.Start();
            this.BeginGetContext();
            this.BeginGetContext();
            this.BeginGetContext();
        }

        protected override void StopImpl()
        {
            _listener.Stop();
        }

        protected override void Dispose(Boolean disposing)
        {
            this._listener.Close();
            base.Dispose(disposing);
        }

        private void BeginGetContext()
        {
            this._listener.BeginGetContext(this.OnRequested, null);
        }

        private void OnRequested(IAsyncResult asyncResult)
        {
            try
            {
                this.HandleRequest(this._listener.EndGetContext(asyncResult));
            }
            finally
            {
                this.BeginGetContext();
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            try
            {
                if (context.Request.Url.PathAndQuery == "/")
                {
                    SendResponse(context.Response, GetContentType(
                        String.Format(
                            ServerResources.HtmlTemplate,
                            String.Format(
                                Resources.IndexPage,
                                context.Request.Url.Host,
                                context.Request.Url.Port
                            ),
                            "MetaTweet HTTP Service",
                            ThisAssembly.EntireCommitId,
                            context.Request.Url.Host,
                            context.Request.Url.Port
                        )
                    ));
                }
                if (context.Request.Url.PathAndQuery.StartsWithAny("/$", "/!"))
                {
                    SendResponse(context.Response, GetContentType(
                        RequestToServer(context.Request.Url.PathAndQuery)
                    ));
                }
                else if (context.Request.Url.PathAndQuery.StartsWith("/res/"))
                {
                    SendResponse(context.Response, GetContentType(
                        Resources.ResourceManager.GetObject(
                            context.Request.Url.PathAndQuery.Substring(5).Replace('.', '_')
                        )
                    ));
                }
            }
            catch (Exception ex)
            {
                SendResponse(context.Response, GetContentType(
                    String.Format(
                        ServerResources.HtmlTemplate,
                        String.Format(
                            "<h1>{0}</h1><p>{1}</<pre>{2}</pre>",
                            ex.GetType().FullName,
                            ex.Message,
                            ex.StackTrace
                        ),
                        "Exception caught",
                        ThisAssembly.EntireCommitId,
                        context.Request.Url.Host,
                        context.Request.Url.Port
                    )
                ));
            }
            finally
            {
                this.BeginGetContext();
            }
        }

        private Object RequestToServer(String requestString)
        {
            return this.Host.Request(Request.Parse(requestString));
        }

        private Tuple<String, Byte[]> GetContentType(Object obj)
        {
            if (obj is String)
            {
                String str = obj as String;
                return Make.Tuple(
                    str.StartsWith("<?xml")
                        ? str.Contains("<html")
                              ? "application/xhtml+xml"
                              : "application/xml"
                        : "text/plain",
                    Encoding.UTF8.GetBytes(str)
                );
            }
            else if (obj is Bitmap)
            {
                return Make.Tuple(
                    ImageCodecInfo.GetImageDecoders()
                        .Single(c => c.FormatID == (obj as Bitmap).RawFormat.Guid)
                        .MimeType,
                    _imageConverter.ConvertTo(obj, typeof(Byte[])) as Byte[]
                );
            }
            else if (obj == null)
            {
                throw new NullReferenceException("Returned null reference.");
            }
            else
            {
                throw new NotSupportedException("To output " + obj.GetType().FullName + " is not supported.");
            }
        }

        private static void SendResponse(HttpListenerResponse response, Tuple<String, Byte[]> data)
        {
            response.ContentType = data.Item1;
            response.ContentLength64 = data.Item2.LongLength;
            response.Close(data.Item2, true);
        }
    }
}