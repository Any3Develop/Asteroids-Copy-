using System;

namespace Services.AbstractFactoryService
{
    public interface IAbstractFactory
    {
        T Create<T>(params object[] args);
        object Create(Type concreteType, params object[] args);
    }
}