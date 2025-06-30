using EmpMovementTracker.Models;

namespace EmpMovementTracker.Services
{
    public class BaseService
    {
        protected readonly TurnstileDbContext context;  

        public BaseService(TurnstileDbContext context)
        {
            this.context = context;
        }
    }
}