using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Core.Domain;

namespace Bifrost.ClientAPI.Controllers.Services
{
  public interface ISecurityService
  {
    Consumer GetConsumer(string apiKey);

  }
}
