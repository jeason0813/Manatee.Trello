﻿namespace Manatee.Trello.Json
{
	public interface IJsonCustomDropDownOption : IJsonCacheable
	{
		IJsonCustomField Field { get; set; }
		string Text { get; set; }
		LabelColor? Color { get; set; }
		IJsonPosition Pos { get; set; }
	}
}