﻿using ByteBank.Forum.App_Start.Identity;
using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Data.Entity;

[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]

namespace ByteBank.Forum
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.CreatePerOwinContext<DbContext>(() =>
                new IdentityDbContext<UsuarioAplicacao>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<UsuarioAplicacao>>((opcoes, contextoOwin) =>
            {
                var dbContext = contextoOwin.Get<DbContext>();
                return new UserStore<UsuarioAplicacao>(dbContext);
            });

            builder.CreatePerOwinContext<UserManager<UsuarioAplicacao>>((opcoes, contextoOwin) =>
            {
                var userStore = contextoOwin.Get<IUserStore<UsuarioAplicacao>>();
                var userManager = new UserManager<UsuarioAplicacao>(userStore);

                var userValidator = new UserValidator<UsuarioAplicacao>(userManager);
                userValidator.RequireUniqueEmail = true;

                userManager.UserValidator = userValidator;
                userManager.PasswordValidator = new SenhaValidador()
                {
                    TamanhoRequerido = 6,
                    ObrigatorioCaracteresEspeciais = true,
                    ObrigatorioDigitos = true,
                    ObrigatorioLowerCase = true,
                    ObrigatorioUpperCase = true
                };

                userManager.EmailService = new EmailServico();

                var dataProtectionProvider = opcoes.DataProtectionProvider;
                var dataProtectionProviderCreated = dataProtectionProvider.Create("ByteBank.Forum");

                userManager.UserTokenProvider = new DataProtectorTokenProvider<UsuarioAplicacao>(dataProtectionProviderCreated);

                userManager.MaxFailedAccessAttemptsBeforeLockout = 3;
                // Máximo de tentativas falhas antes de bloquear usuário
                userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                // Usuário ficara impedido por 5 minutos de efetuar o login
                userManager.UserLockoutEnabledByDefault = true;
                // Bloqueia todos os usuários

                return userManager;
            });

            builder.CreatePerOwinContext<SignInManager<UsuarioAplicacao, string>>((opcoes, contextoOwin) =>
            {
                var userManager = contextoOwin.Get<UserManager<UsuarioAplicacao>>();
                var signInManager =
                    new SignInManager<UsuarioAplicacao, string>(
                        userManager,
                        contextoOwin.Authentication
                    );

                return signInManager;
            });

            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}