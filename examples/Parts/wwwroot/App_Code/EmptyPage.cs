using N2;
using N2.Details;
using N2.Integrity;

/// <summary>
/// This is my custom content data container class. Since this class derives 
/// from <see cref="N2.ContentItem"/> N2 will pick up this class and make it 
/// available when we create pages in edit mode. 
/// 
/// Another thing to notice is that in addition to Text defined to be editable 
/// in this class it's Title and Name are also editable. This is because of the 
/// abstract base class <see cref="AbstractPage"/> it derives from.
/// </summary>
[Definition("Empty page", "EmptyPage")]
[AvailableZone("Content", "Content")] // <-- This tells N2 that there is a zone called "Content"
public class EmptyPage : AbstractPage
{
}
