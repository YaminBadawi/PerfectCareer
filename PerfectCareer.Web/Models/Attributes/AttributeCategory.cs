using System.ComponentModel.DataAnnotations;

namespace PerfectCareer.Web.Models.Attributes;

public enum AttributeCategory
{
    Certification = 1,

    [Display(Name = "Domain Knowledge")]
    DomainKnowledge = 2,

    [Display(Name = "Personal Information")]
    PersonalInformation = 3,

    [Display(Name = "Soft Skills")]
    SoftSkills = 4
}