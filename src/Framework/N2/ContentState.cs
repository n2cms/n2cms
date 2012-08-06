namespace N2
{
    /// <summary>
    /// The current state of a content item
    /// </summary>
	public enum ContentState
	{
		/// <summary>The item's state is not known.</summary>
		None = 0,
		/// <summary>The item is new and has not been saved yet.</summary>
		New = 1,
		/// <summary>The item is a draft that has not been published.</summary>
		Draft = 2,
		/// <summary>The item is waiting for the opportune moment to be published.</summary>
		Waiting = 4,
		/// <summary>The item is published.</summary>
		Published = 16,
		/// <summary>The item was previously published.</summary>
		Unpublished = 32,
		/// <summary>The item is deleted and resides in the trash can.</summary>
		Deleted = 64,
	}
}
