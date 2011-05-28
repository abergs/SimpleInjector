﻿namespace SimpleInjector.CodeSamples
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ConstructorRegistrationExtensions
    {
        public static void RegisterWithConstructor<TService>(this Container container, 
            Expression<Func<TService>> constructorCall)
            where TService : class
        {
            var ctor = ((NewExpression)constructorCall.Body).Constructor;

            var producer = new Transient<TService>(container, ctor);

            container.Register<TService>(producer.GetInstance);
        }

        [DebuggerStepThrough]
        private static Func<TService> Build<TService>(Container container, 
            ConstructorInfo constructor)
        {
            var parameters =
                from parameter in constructor.GetParameters()
                select BuildParameter(container, parameter.ParameterType);

            var newExpression = Expression.Lambda<Func<TService>>(
                Expression.New(constructor, parameters.ToArray()), 
                new ParameterExpression[0]);

            return newExpression.Compile();
        }

        [DebuggerStepThrough]
        private static Expression BuildParameter(Container container,
            Type parameterType)
        {
            var instanceProducer = container.GetRegistration(parameterType);

            if (instanceProducer == null)
            {
                container.GetInstance(parameterType);
            }

            return instanceProducer.BuildExpression();
        }

        private sealed class Transient<TService>
        {
            private readonly Container container;
            private readonly ConstructorInfo ctor;

            private Func<TService> instanceCreator;

            public Transient(Container container, ConstructorInfo ctor)
            {
                this.container = container;
                this.ctor = ctor;
            }

            [DebuggerStepThrough]
            public TService GetInstance()
            {
                if (this.instanceCreator == null)
                {
                    this.instanceCreator = Build<TService>(this.container, this.ctor);
                }

                return this.instanceCreator();
            }
        }
    }
}