// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;

namespace TokenServiceAPI
{
    public class Config
    {
        
        public static Dictionary<string, string> GetUrls(IConfiguration configuration)
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();

            urls.Add("Mvc", configuration.GetValue<string>("MvcClient"));

            return urls;

        }

       public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                 new ApiResource("cart", "Cart Management API") { Scopes = { "cart" } }
            };
        }  

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[] { new ApiScope("cart", "Cart Management API") };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
               // new IdentityResources.Email()
            };
        }

        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientUrls)
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = new [] { new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    // The existing MVC client uses code + id_token (hybrid), without PKCE.
                    RequirePkce = false,

                    RedirectUris = {$"{clientUrls["Mvc"]}/signin-oidc"},
                    PostLogoutRedirectUris = {$"{clientUrls["Mvc"]}/signout-callback-oidc"},
                    AllowAccessTokensViaBrowser = false,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                      //  IdentityServerConstants.StandardScopes.Email,
                         "cart"
                    }

                }
            };
        }

    }
}
