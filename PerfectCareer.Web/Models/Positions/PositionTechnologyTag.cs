using PerfectCareer.Web.Models.Projects;

namespace PerfectCareer.Web.Models.Positions;

public sealed class PositionTechnologyTag
{
    public int PositionId { get; set; }

    public int TechnologyTagId { get; set; }

    public Position Position { get; set; } = null!;

    public TechnologyTag TechnologyTag { get; set; } = null!;
}