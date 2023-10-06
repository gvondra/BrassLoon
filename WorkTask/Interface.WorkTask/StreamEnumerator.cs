using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    internal class StreamEnumerator<TProto, TModel> : IAsyncEnumerator<TModel>
    {
        private readonly GrpcChannel _channel;
        private readonly Func<GrpcChannel, Task<AsyncServerStreamingCall<TProto>>> _openStreamingCall;
        private readonly Func<TProto, TModel> _map;
        private AsyncServerStreamingCall<TProto> _serverStreamingCall;

        internal StreamEnumerator(
            GrpcChannel channel,
            Func<GrpcChannel, Task<AsyncServerStreamingCall<TProto>>> openStreamingCall,
            Func<TProto, TModel> map)
        {
            _channel = channel;
            _openStreamingCall = openStreamingCall;
            _map = map;
        }

        public TModel Current { get; private set; }

        public ValueTask DisposeAsync()
        {
            if (_channel != null)
                _channel.Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_serverStreamingCall == null)
                _serverStreamingCall = await _openStreamingCall(_channel);
            bool result = await _serverStreamingCall.ResponseStream.MoveNext(CancellationToken.None);
            if (result)
                Current = _map(_serverStreamingCall.ResponseStream.Current);
            return result;
        }
    }
}
