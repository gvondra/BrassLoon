using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LogRPC.Protos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace LogRPC.Services
{
    public class TraceService : Protos.TraceService.TraceServiceBase
    {
        [Authorize]
        public override async Task<Empty> Create(IAsyncStreamReader<Trace> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                Console.WriteLine("process Trace here");
            }
            return new Empty();
        }
    }
}
