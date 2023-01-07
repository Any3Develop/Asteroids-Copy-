using System;
using Services.EntityService;

namespace Asterodis.Entities.Players
{
    public interface IAiTargetSceneEntity : ISceneEntity, IDisposable
    {
        bool IsValidTarget { get; }
    }
}