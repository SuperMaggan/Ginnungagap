using System;
using System.Collections.Generic;
using System.Security;
using Bifrost.ClientAPI.Controllers.Services;
using Bifrost.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost.ClientAPI.Controllers
{
  public abstract class AsgardController : Controller
  {
    protected readonly ISecurityService SecurityService;

    protected AsgardController(ISecurityService securityService)
    {
      SecurityService = securityService;
    }


    protected string GetApiKey()
    {
      if (!Request.Headers.TryGetValue("apikey", out var apiKey))
        throw new Exception("Missing a valid apikey header");
      if (string.IsNullOrEmpty(apiKey))
        throw new SecurityException("No value in the apikey header!");
      return apiKey;
    }


   

    protected void EnsureAdmin()
    {
      var consumer = SecurityService.GetConsumer(GetApiKey());
      if (!consumer.IsAdmin)
        throw new UnauthorizedAccessException("You need to be an administrator to use this");
    }

    protected Consumer EnsureEditAccess()
    {
      var consumer = GetConsumer();
      if (!consumer.IsAdmin && !consumer.CanEdit)
        throw new SecurityException("Your consumer don't have edit rights");
      return consumer;
    }

    protected Consumer GetConsumer()
    {
      return SecurityService.GetConsumer(GetApiKey());
    }



  }
}