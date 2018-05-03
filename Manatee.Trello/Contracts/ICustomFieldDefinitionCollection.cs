﻿using System.Threading;
using System.Threading.Tasks;

namespace Manatee.Trello
{
	/// <summary>
	/// A collection of custom field definitions.
	/// </summary>
	public interface ICustomFieldDefinitionCollection : IReadOnlyCollection<ICustomFieldDefinition>
	{
		/// <summary>
		/// Adds a new custom field definition to a board.
		/// </summary>
		/// <param name="name">The field's name.</param>
		/// <param name="type">The field's data type.</param>
		/// <param name="ct">(Optional) A cancellation token for async processing.</param>
		/// <param name="options">(Optional) A collection of drop down options.</param>
		/// <returns>The <see cref="ICustomFieldDefinition"/> generated by Trello.</returns>
		Task<ICustomFieldDefinition> Add(string name, CustomFieldType type,
		                                 CancellationToken ct = default(CancellationToken),
		                                 params IDropDownOption[] options);
	}
}