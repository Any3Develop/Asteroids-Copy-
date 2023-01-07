using System;
using Zenject;

namespace Services.AbstractFactoryService
{
    public class AbstractFactory : IAbstractFactory
    {
        private readonly IInstantiator instantiator;

        public AbstractFactory (IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public T Create<T>(params object[] args)
        {
            var obj = instantiator.Instantiate<T>(args);
            Initialize(obj);
            return obj;
        }

        public object Create(Type concreteType, params object[] args)
        {
            var obj = instantiator.Instantiate(concreteType, args);
            Initialize(obj);
            return obj;
        }

        private void Initialize<T>(T obj)
        {
            if (obj is IInitializable initializable)
                initializable.Initialize();
        }
    }
}