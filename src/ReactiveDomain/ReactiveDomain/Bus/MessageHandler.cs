// Copyright (c) 2012, Event Store LLP
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
// 
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.
// Neither the name of the Event Store LLP nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.Bus
{
    public interface IMessageHandler
    {
        string HandlerName { get; }
        int MessageTypeId { get; }
        bool TryHandle(Message message);
        bool IsSame<T>(object handler);
    }
    
    internal class MessageHandler<T> : IMessageHandler where T : Message
    {
        public string HandlerName { get; private set; }

        public int MessageTypeId { get; private set; }

        private readonly IHandle<T> _handler;

        internal MessageHandler(IHandle<T> handler, string handlerName, int messageTypeId)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            _handler = handler;
            HandlerName = handlerName ?? string.Empty;
            MessageTypeId = messageTypeId;
        }

        public bool TryHandle(Message message)
        {
            var msg = message as T;
            if (msg == null) return false;

            _handler.Handle(msg); //if this throws let it bubble up.
            return true;
        }

        public bool IsSame<T2>(object handler)
        {
            return ReferenceEquals(_handler, handler) && typeof(T) == typeof(T2);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(HandlerName) ? _handler.ToString() : HandlerName;
        }
    }
}