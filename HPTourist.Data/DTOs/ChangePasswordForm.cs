using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace HPTourist.Data.DTOs
{
	public class ChangePasswordForm
	{
		[Required]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
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
