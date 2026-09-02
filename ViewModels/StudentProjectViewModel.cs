using MachineShopManager.Models;

namespace MachineShopManager.ViewModels;

public sealed class StudentProjectViewModel
{
    public required Project Project { get; init; }

    // Calculated from the active queue on every consultation; never persisted.
    public int? QueuePosition { get; init; }

    public int? QueueTotal { get; init; }
}
