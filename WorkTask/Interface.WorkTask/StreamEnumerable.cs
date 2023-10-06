using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    internal class StreamEnumerable<TProto, TModel> : IAsyncEnumerable<TModel>
    {
        private readonly ISettings _settings;
        private readonly Func<GrpcChannel, Task<AsyncServerStreamingCall<TProto>>> _openStreamingCall;
        private readonly Func<TProto, TModel> _map;

        public StreamEnumerable(
            ISettings settings,
            Func<GrpcChannel, Task<AsyncServerStreamingCall<TProto>>> openStreamingCall,
            Func<TProto, TModel> map)
        {
            _settings = settings;
            _openStreamingCall = openStreamingCall;
            _map = map;
        }

        public IAsyncEnumerator<TModel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new StreamEnumerator<TProto, TModel>(
                GrpcChannel.ForAddress(_settings.BaseAddress),
                _openStreamingCall,
                _map);
        }
    }
}
