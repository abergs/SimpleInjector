﻿namespace SimpleInjector.CodeSamples.Tests.Unit
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SimpleInjector.Advanced;
    using SimpleInjector.Extensions;

    [TestClass]
    public class RuntimeDecoratorExtensionsTests
    {
        [TestMethod]
        public void GetInstance_OnRegisterRuntimeDecoratorRegistration_DecorationCanBeChangedDynamically()
        {
            // Arrange
            var container = new Container();

            bool decorateHandler = false;

            container.Register<ICommandHandler<RealCommand>, NullCommandHandler<RealCommand>>();

            container.RegisterRuntimeDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerDecorator<>),
                c => decorateHandler);

            // Act
            var handler1 = container.GetInstance<ICommandHandler<RealCommand>>();

            // Runtime switch
            decorateHandler = true;

            var handler2 = container.GetInstance<ICommandHandler<RealCommand>>();

            // Assert
            Assert.IsInstanceOfType(handler1, typeof(NullCommandHandler<RealCommand>));
            Assert.IsInstanceOfType(handler2, typeof(CommandHandlerDecorator<RealCommand>));
        }

        [TestMethod]
        public void GetInstance_OnRegisterSingletonRuntimeDecoratorRegistration_CreatesTheDecoratorAsSingleton()
        {
            // Arrange
            var container = new Container();

            bool decorateHandler = false;

            container.Register<ICommandHandler<RealCommand>, NullCommandHandler<RealCommand>>();

            container.RegisterRuntimeDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerDecorator<>),
                Lifestyle.Singleton, c => decorateHandler);

            // Act
            var handler1 = container.GetInstance<ICommandHandler<RealCommand>>();
            var handler2 = container.GetInstance<ICommandHandler<RealCommand>>();

            // Runtime switch
            decorateHandler = true;

            var handler3 = container.GetInstance<ICommandHandler<RealCommand>>();
            var handler4 = container.GetInstance<ICommandHandler<RealCommand>>();

            // Assert
            Assert.AreNotSame(handler1, handler2);
            Assert.AreSame(handler3, handler4);
        }

        [TestMethod]
        public void GetInstance_OnRegisterRuntimeDecoratorRegistrationAndSingletonProxy_DecorationCanBeChangedDynamically()
        {
            // Arrange
            var container = new Container();

            bool decorateHandler = false;

            container.Register<ICommandHandler<RealCommand>, NullCommandHandler<RealCommand>>();

            container.RegisterRuntimeDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerDecorator<>),
                c => decorateHandler);

            container.RegisterSingleDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerProxy<>));

            // Act
            var handler1 = 
                ((CommandHandlerProxy<RealCommand>)container.GetInstance<ICommandHandler<RealCommand>>())
                .DecorateeFactory();

            // Runtime switch
            decorateHandler = true;

            var handler2 = 
                ((CommandHandlerProxy<RealCommand>)container.GetInstance<ICommandHandler<RealCommand>>())
                .DecorateeFactory();

            // Assert
            Assert.IsInstanceOfType(handler1, typeof(NullCommandHandler<RealCommand>));
            Assert.IsInstanceOfType(handler2, typeof(CommandHandlerDecorator<RealCommand>));
        }

        [TestMethod]
        public void GetInstance_OnRegisterRuntimeDecoratorRegistrationWithOverriddenLifestyleSelectionBehavior_CreatesTheDecoratorWithExpectedLifestyler()
        {
            // Arrange
            var container = new Container();

            container.Options.LifestyleSelectionBehavior = 
                new CustomLifestyleSelectionBehavior(Lifestyle.Singleton);

            container.Register<ICommandHandler<RealCommand>, NullCommandHandler<RealCommand>>();

            container.RegisterRuntimeDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerDecorator<>),
                c => true);

            // Act
            var handler1 = container.GetInstance<ICommandHandler<RealCommand>>();
            var handler2 = container.GetInstance<ICommandHandler<RealCommand>>();

            // Assert
            Assert.AreSame(handler1, handler2);
        }

        private sealed class CustomLifestyleSelectionBehavior : ILifestyleSelectionBehavior
        {
            private readonly Lifestyle lifestyle;

            public CustomLifestyleSelectionBehavior(Lifestyle lifestyle)
            {
                this.lifestyle = lifestyle;
            }

            public Lifestyle SelectLifestyle(Type serviceType, Type implementationType)
            {
                return this.lifestyle;
            }
        }
    }
}