using System;
using System.Web.Http;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(WebChat.Services.App_Start.CorsConfig), "PreStart")]

namespace WebChat.Services.App_Start {
    public static class CorsConfig {
        public static void PreStart() {
            GlobalConfiguration.Configuration.MessageHandlers.Add(new RedRocket.WebApi.Cors.CorsHandler());
        }
    }
}

