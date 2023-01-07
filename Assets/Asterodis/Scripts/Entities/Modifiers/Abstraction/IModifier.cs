using System;

namespace Asterodis.Entities.Modifiers
{
    public interface IModifier : IDisposable
    {
        void Apply(string ownerId);
    }
}