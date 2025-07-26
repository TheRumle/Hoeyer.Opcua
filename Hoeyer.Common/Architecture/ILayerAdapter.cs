using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Common.Architecture;

public interface ILayerAdapter<in TAdaptionSource>
{
    public void Adapt(TAdaptionSource adaptionSource, IServiceCollection adaptionTarget);
}