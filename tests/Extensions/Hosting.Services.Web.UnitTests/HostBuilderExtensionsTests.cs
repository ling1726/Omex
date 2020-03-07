﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Fabric;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Omex.Extensions.Hosting.Services;
using Microsoft.Omex.Extensions.Hosting.Services.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceFabric.Mocks;

namespace Hosting.Services.Web.UnitTests
{
	[TestClass]
	public class HostBuilderExtensionsTests
	{
		[TestMethod]
		public void CheckTypeRegistrationForStateless() =>
			CheckTypeRegistration(
				MockStatelessServiceContextFactory.Default,
				(v, h) => h.BuildStatelessService(
					"StatelessServiceName",
					b => b.AddKestrelListener<ListenerValidator.Startup>(v.ListenerName, v.IntegrationOptions, v.BuilderAction)));


		[TestMethod]
		public void CheckTypeRegistrationForStateful() =>
			CheckTypeRegistration(
				MockStatefulServiceContextFactory.Default,
				(v, h) => h.BuildStatefulService(
					"StatefulServiceName",
					b => b.AddKestrelListener<ListenerValidator.Startup>(v.ListenerName, v.IntegrationOptions, v.BuilderAction)));


		private void CheckTypeRegistration<TContext>(
			TContext context,
			Func<ListenerValidator, IHostBuilder, IHost> buildAction)
			where TContext : ServiceContext
		{
			ListenerValidator validator = new ListenerValidator();

			IHostBuilder hostBuilder = new HostBuilder()
				.UseDefaultServiceProvider(options =>
				{
					options.ValidateOnBuild = true;
					options.ValidateScopes = true;
				});

			IHost host = buildAction(validator, hostBuilder);
			IListenerBuilder<TContext> builder = host.Services.GetService<IListenerBuilder<TContext>>();

			Assert.IsNotNull(builder);

			KestrelListenerBuilder<ListenerValidator.Startup, TContext> kestrelBuilder
				= (KestrelListenerBuilder<ListenerValidator.Startup, TContext>)builder;

			IWebHost webHost = validator.ValidateListenerBuilder(context, kestrelBuilder);
			validator.ValidateOmexTypesRegistred(webHost);
			validator.ValidateBuildFunction(context, kestrelBuilder);
		}
	}
}