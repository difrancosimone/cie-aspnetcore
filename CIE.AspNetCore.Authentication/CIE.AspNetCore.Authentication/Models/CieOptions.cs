﻿using CIE.AspNetCore.Authentication.Events;
using CIE.AspNetCore.Authentication.Helpers;
using CIE.AspNetCore.Authentication.Models.ServiceProviders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CIE.AspNetCore.Authentication.Models
{
    public class CieOptions : RemoteAuthenticationOptions
    {
        private readonly List<ServiceProvider> _spMetadata = new();

        public CieOptions()
        {
            CallbackPath = "/signin-cie";
            // In ADFS the cleanup messages are sent to the same callback path as the initial login.
            // In AAD it sends the cleanup message to a random Reply Url and there's no deterministic way to configure it.
            //  If you manage to get it configured, then you can set RemoteSignOutPath accordingly.
            RemoteSignOutPath = "/signout-cie";
            ServiceProvidersMetadataEndpointsBasePath = "/metadata-cie";
            Events = new CieEvents();
        }

        /// <summary>
        /// Check that the options are valid.  Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            base.Validate();

        }

        /// <summary>
        ///  Requests received on this path will cause the handler to invoke SignOut using the SignOutScheme.
        /// </summary>
        public PathString RemoteSignOutPath { get; set; }

        /// <summary>
        /// Indicates if requests to the CallbackPath may also be for other components. If enabled the handler will pass
        /// requests through that do not contain Cie authentication responses. Disabling this and setting the
        /// CallbackPath to a dedicated endpoint may provide better error handling.
        /// This is disabled by default.
        /// </summary>
        public bool SkipUnrecognizedRequests { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CieEvents"/> to call when processing Cie messages.
        /// </summary>
        public new CieEvents Events
        {
            get => (CieEvents)base.Events;
            set => base.Events = value;
        }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        /// <summary>
        /// Indicates that the authentication session lifetime (e.g. cookies) should match that of the authentication token.
        /// If the token does not provide lifetime information then normal session lifetimes will be used.
        /// This is enabled by default.
        /// </summary>
        public bool UseTokenLifetime { get; set; } = true;

        /// <summary>
        /// The Ws-Federation protocol allows the user to initiate logins without contacting the application for a Challenge first.
        /// However, that flow is susceptible to XSRF and other attacks so it is disabled here by default.
        /// </summary>
        public bool AllowUnsolicitedLogins { get; set; }

        /// <summary>
        /// The Authentication Scheme to use with SignOutAsync from RemoteSignOutPath. SignInScheme will be used if this
        /// is not set.
        /// </summary>
        public string SignOutScheme { get; set; }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the index of the assertion consumer service.
        /// </summary>
        /// <value>
        /// The index of the assertion consumer service.
        /// </value>
        public ushort AssertionConsumerServiceIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the attribute consuming service.
        /// </summary>
        /// <value>
        /// The index of the attribute consuming service.
        /// </value>
        public ushort AttributeConsumingServiceIndex { get; set; }

        /// <summary>
        /// Gets the identity providers.
        /// </summary>
        /// <value>
        /// The identity providers.
        /// </value>
        public IdentityProvider IdentityProvider { get; set; }

        /// <summary>
        /// Gets or sets the certificate.
        /// </summary>
        /// <value>
        /// The certificate.
        /// </value>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether idp metadata should be cached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cache idp metadata]; otherwise, <c>false</c>.
        /// </value>
        public bool CacheIdpMetadata { get; set; }

        /// <summary>
        /// Gets or sets the type of the principal name claim.
        /// </summary>
        /// <value>
        /// The type of the principal name claim.
        /// </value>
        public CieClaimTypes PrincipalNameClaimType { get; set; } = CieClaimTypes.FiscalNumber;

        /// <summary>
        /// Gets or sets the base path where the configured SP metadata will be exposed.
        /// </summary>
        /// <value>
        /// The SP Metadata Endpoints BasePath.
        /// </value>
        public PathString ServiceProvidersMetadataEndpointsBasePath { get; set; }

        /// <summary>
        /// Gets or sets the security level.
        /// </summary>
        /// <value>
        /// The security level.
        /// </value>
        public int SecurityLevel { get; set; }
        public RequestMethod RequestMethod { get; set; }

        /// <summary>
        /// Gets or sets the collection of the exposed SP metadata.
        /// </summary>
        /// <value>
        /// The collection of the exposed SP metadata.
        /// </value>
        public List<ServiceProvider> ServiceProviders { get { return _spMetadata; } }

        public void LoadFromConfiguration(IConfiguration configuration)
        {
            var conf = OptionsHelper.CreateFromConfiguration(configuration);
            IdentityProvider = conf.IdentityProvider;
            AllowUnsolicitedLogins = conf.AllowUnsolicitedLogins;
            AssertionConsumerServiceIndex = conf.AssertionConsumerServiceIndex;
            AttributeConsumingServiceIndex = conf.AttributeConsumingServiceIndex;
            CallbackPath = conf.CallbackPath.HasValue ? conf.CallbackPath : CallbackPath;
            EntityId = conf.EntityId;
            RemoteSignOutPath = conf.RemoteSignOutPath.HasValue ? conf.RemoteSignOutPath : RemoteSignOutPath;
            SignOutScheme = conf.SignOutScheme;
            UseTokenLifetime = conf.UseTokenLifetime;
            SkipUnrecognizedRequests = conf.SkipUnrecognizedRequests;
            Certificate = conf.Certificate;
            CacheIdpMetadata = conf.CacheIdpMetadata;
            RequestMethod = conf.RequestMethod;
            SecurityLevel = conf.SecurityLevel;
        }
    }
}
