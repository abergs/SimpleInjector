﻿#region Copyright Simple Injector Contributors
/* The Simple Injector is an easy-to-use Inversion of Control library for .NET
 * 
 * Copyright (c) 2013-2014 Simple Injector Contributors
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO 
 * EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

namespace SimpleInjector.Integration.Wcf
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.ServiceModel;
    using System.Threading;

    /// <summary>
    /// Thread and container specific cache for services that are registered with one of the 
    /// <see cref="SimpleInjectorWcfExtensions">RegisterPerWcfOperation</see> extension method overloads.
    /// This class is created implicitly and a current instance can be requested by calling
    /// <see cref="SimpleInjectorWcfExtensions.GetCurrentWcfOperationScope">GetCurrentWcfOperationScope</see>.
    /// </summary>
    public sealed class WcfOperationScope : Scope
    {
        private InstanceContext instanceContext;

        internal WcfOperationScope(InstanceContext instanceContext)
        {
            this.instanceContext = instanceContext;
        }

        /// <summary>Releases all instances that are cached by the <see cref="WcfOperationScope"/> object.</summary>
        /// <param name="disposing">False when only unmanaged resources should be released.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.instanceContext == null)
            {
                return;
            }

            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                try
                {
                    this.instanceContext.RemoveScope();
                }
                finally
                {
                    this.instanceContext = null;
                }
            }
        }
    }
}