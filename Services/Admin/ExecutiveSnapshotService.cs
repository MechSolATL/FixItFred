using System.Threading.Tasks;
using MVP_Core.Pages.Admin;

namespace MVP_Core.Services.Admin
{
    public class ExecutiveSnapshotService
    {
        public Task<ExecutiveSnapshotViewModel> GetSnapshotAsync()
        {
            // TODO: Implement real aggregation logic
            return Task.FromResult(new ExecutiveSnapshotViewModel());
        }
    }
}
