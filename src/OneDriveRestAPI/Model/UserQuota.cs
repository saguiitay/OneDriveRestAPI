namespace OneDriveRestAPI.Model
{
	public class UserQuota
	{
		/// <summary>
		/// The user's remaining unused storage space, in bytes.
		/// </summary>
		public long Available { get; set; }

		/// <summary>
		/// The user's total available storage space, in bytes
		/// </summary>
		public long Quota { get; set; }
	}
}
