using PerfectCareer.Web.Models.Attributes;

namespace PerfectCareer.Web.Models.Positions;

public sealed class PositionTemplateAttribute
{
    public int PositionId { get; set; }

    public int AttributeDefinitionId { get; set; }

    public int DisplayOrder { get; set; }

    public Position Position { get; set; } = null!;

    public AttributeDefinition AttributeDefinition { get; set; } = null!;
}