using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace HPTourist.Data.DTOs
{
	public class ChangePasswordForm
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; } = string.Empty;

		[Required]
		[Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		public string NewPasswordConfirm { get; set; } = string.Empty;
	}
}
