using MudBlazor;

namespace EmpMovementTracker.Services
{
    public class LayoutService
    {
        // Breadcrumbs
        private IEnumerable<BreadcrumbItem> breadcrumbs = [];
        public IEnumerable<BreadcrumbItem> Breadcrumbs => breadcrumbs;

        public event Action? OnBreadcrumbsChanged;

        public LayoutService()
        {
        }

        public void SetBreadcrumbs(IEnumerable<BreadcrumbItem> items)
        {
            breadcrumbs = items?.ToList() ?? [];
            OnBreadcrumbsChanged?.Invoke();
        }

        public void ClearBreadcrumbs()
        {
            breadcrumbs = [];
            OnBreadcrumbsChanged?.Invoke();
        }
    }
}