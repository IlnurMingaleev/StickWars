using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UI
{
    public interface IPreloadResourcesAsync
    {
        public UniTask PreloadResources(IProgress<float> progress, CancellationToken cancellationToken);
    }
}