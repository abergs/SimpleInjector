﻿namespace SimpleInjector.Tests.Unit.Advanced
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultConstructorVerificationBehaviorTests
    {
        [TestMethod]
        public void Verify_TValueTypeParameter_ThrowsExpectedException()
        {
            // Arrange
            string expectedString = @"
                The constructor of type 
                DefaultConstructorVerificationBehaviorTests+TypeWithSinglePublicConstructorWithValueTypeParameter 
                contains parameter 'intArgument' of type Int32 which can not be used for constructor injection 
                because it is a value type.
                ".TrimInside();

            var behavior = new ContainerOptions().ConstructorVerificationBehavior;

            var constructor =
                typeof(TypeWithSinglePublicConstructorWithValueTypeParameter).GetConstructors().Single();

            var invalidParameter = constructor.GetParameters().Single();

            try
            {
                // Act
                behavior.Verify(invalidParameter);

                // Assert
                Assert.Fail("Exception expected.");
            }
            catch (ActivationException ex)
            {
                AssertThat.StringContains(expectedString, ex.Message);
            }
        }

        [TestMethod]
        public void Verify_StringTypeParameter_ThrowsExpectedException()
        {
            // Arrange
            string expectedString = @"
                The constructor of type 
                DefaultConstructorVerificationBehaviorTests+TypeWithSinglePublicConstructorWithStringTypeParameter 
                contains parameter 'stringArgument' of type String which can not be used for constructor 
                injection."
                .TrimInside();

            var behavior = new ContainerOptions().ConstructorVerificationBehavior;

            var constructor =
                typeof(TypeWithSinglePublicConstructorWithStringTypeParameter).GetConstructors().Single();

            var invalidParameter = constructor.GetParameters().Single();

            try
            {
                // Act
                behavior.Verify(invalidParameter);

                // Assert
                Assert.Fail("Exception expected.");
            }
            catch (ActivationException ex)
            {
                AssertThat.StringContains(expectedString, ex.Message);
            }
        }

        private class TypeWithSinglePublicConstructorWithValueTypeParameter
        {
            public TypeWithSinglePublicConstructorWithValueTypeParameter(int intArgument)
            {
            }
        }

        private class TypeWithSinglePublicConstructorWithStringTypeParameter
        {
            public TypeWithSinglePublicConstructorWithStringTypeParameter(string stringArgument)
            {
            }
        }
    }
}
