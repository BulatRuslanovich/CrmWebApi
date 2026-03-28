using System.ComponentModel.DataAnnotations;

namespace CrmWebApi.DTOs.Spec;

public record CreateSpecRequest(
	[Required, MaxLength(200)] string SpecName
);
