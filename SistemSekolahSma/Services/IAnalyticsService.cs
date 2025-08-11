using SistemSekolahSMA.ViewModels;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Services
{
    public interface IAnalyticsService
    {
        Task<ViewModels.DashboardAnalytics> GetDashboardAnalyticsAsync();
    }
}