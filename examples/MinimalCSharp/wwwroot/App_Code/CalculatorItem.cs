using N2;
using N2.Details;

/// <summary>
/// This part can be dragged onto any DroppableZone on a page.
/// </summary>
[PartDefinition("Calculator", TemplateUrl = "~/UI/Calculator.ascx")]
public class CalculatorItem : MyItemBase
{
	[EditableCheckBox("Enable Add", 100)]
	public virtual bool EnableAdd
	{
		get { return GetDetail("EnableAdd", true); }
		set { SetDetail("EnableAdd", value, true); }
	}

	[EditableCheckBox("Enable Subtract", 101)]
	public virtual bool EnableSubtract
	{
		get { return GetDetail("EnableSubtract", true); }
		set { SetDetail("EnableSubtract", value, true); }
	}

	[EditableCheckBox("Enable Multiply", 102)]
	public virtual bool EnableMultiply
	{
		get { return GetDetail("EnableMultiply", true); }
		set { SetDetail("EnableMultiply", value, true); }
	}

	[EditableCheckBox("Enable Divide", 103)]
	public virtual bool EnableDivide
	{
		get { return GetDetail("EnableDivide", true); }
		set { SetDetail("EnableDivide", value, true); }
	}
}