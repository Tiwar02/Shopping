using Shopping.Common;
using Shopping.Models;

namespace Shopping.Helpers
{
    public interface IOrderHelper 
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel model);
        Task<Response> CancelOrderAsync(int id);
    }
}
