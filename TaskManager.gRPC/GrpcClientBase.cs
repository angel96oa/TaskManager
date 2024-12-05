using Microsoft.Extensions.Logging;
using TaskManager.gRPC;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager.gRPC
{
    public class GrpcClientBase : IDisposable
    {
        private bool disposedValue;
        private readonly ILogger _logger;
        protected GrpcClientBase(ILogger logger)
        {
            _logger = logger;
        }

        //protected async Task<T1> CallField<T1>(Func<JsonMessage, AsyncUnaryCall<JsonMessage>> action)
        //{
        //    T1 t1 = default;
            
        //    await ExecuteTask
        //}

        //private async Task<T1> ExecuteTask<T1>(Task<T1> task)
        //{
        //    try
        //    {
        //        return await task;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //    }
        //}

        //protected async Task<T1> Call

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GrpcClientBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
