﻿namespace SimpleInjector.Diagnostics.Tests.Unit
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SimpleInjector.Diagnostics.Analyzers;
    using SimpleInjector.Diagnostics.Debugger;
    using SimpleInjector.Extensions;
    using SimpleInjector.Extensions.LifetimeScoping;

    [TestClass]
    public class DisposableTransientComponentsTests
    {
        [TestMethod]
        public void Analyze_TransientRegistrationForDisposableComponent_Warning()
        {
            // Arrange
            var container = new Container();

            container.Register<IPlugin, DisposablePlugin>();

            container.Verify();

            // Act
            var results = Analyzer.Analyze(container).OfType<DisposableTransientComponentDiagnosticResult>()
                .ToArray();

            // Assert
            Assert.AreEqual(1, results.Length, Actual(results));
            Assert.AreEqual(
                "DisposablePlugin is registered as transient, but implements IDisposable.",
                results.Single().Description, 
                Actual(results));
        }

        [TestMethod]
        public void Analyze_SingletonRegistrationForDisposableComponent_NoWarning()
        {
            // Arrange
            var container = new Container();

            container.Register<IPlugin, DisposablePlugin>(Lifestyle.Singleton);

            container.Verify();

            // Act
            var results = Analyzer.Analyze(container).OfType<DisposableTransientComponentDiagnosticResult>()
                .ToArray();

            // Assert
            Assert.AreEqual(0, results.Length, Actual(results));
        }

        [TestMethod]
        public void Analyze_ScopedRegistrationForDisposableComponent_NoWarning()
        {
            // Arrange
            var container = new Container();

            container.Register<IPlugin, DisposablePlugin>(new LifetimeScopeLifestyle());

            container.Verify();

            // Act
            var results = Analyzer.Analyze(container).OfType<DisposableTransientComponentDiagnosticResult>()
                .ToArray();

            // Assert
            Assert.AreEqual(0, results.Length, Actual(results));
        }

        [TestMethod]
        public void Analyze_TransientRegistrationForComponentThatsNotDisposable_NoWarning()
        {
            // Arrange
            var container = new Container();

            container.Register<IPlugin, SomePluginImpl>();

            container.Verify();

            // Act
            var results = Analyzer.Analyze(container).OfType<DisposableTransientComponentDiagnosticResult>()
                .ToArray();

            // Assert
            Assert.AreEqual(0, results.Length, Actual(results));
        }

        private static string Actual(DisposableTransientComponentDiagnosticResult[] results)
        {
            return "actual: " + string.Join(" - ", results.Select(r => r.Description));
        }
    }
}