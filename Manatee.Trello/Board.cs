﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.DataAccess;
using Manatee.Trello.Internal.Synchronization;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// Represents a board.
	/// </summary>
	public class Board : IBoard
	{
		/// <summary>
		/// Enumerates the data which can be pulled for boards.
		/// </summary>
		[Flags]
		public enum Fields
		{
			/// <summary>
			/// Indicates the Name property should be populated.
			/// </summary>
			[Display(Description="name")]
			Name = 1,
			/// <summary>
			/// Indicates the Description property should be populated.
			/// </summary>
			[Display(Description="desc")]
			Description = 1 << 1,
			/// <summary>
			/// Indicates the Closed property should be populated.
			/// </summary>
			[Display(Description="closed")]
			Closed = 1 << 2,
			/// <summary>
			/// Indicates the Organization property should be populated.
			/// </summary>
			[Display(Description="idOrganization")]
			Organization = 1 << 3,
			/// <summary>
			/// Indicates the Pinned property should be populated.
			/// </summary>
			[Display(Description="pinned")]
			Pinned = 1 << 4,
			/// <summary>
			/// Indicates the Starred property should be populated.
			/// </summary>
			[Display(Description="starred")]
			Starred = 1 << 5,
			/// <summary>
			/// Indicates the Preferencess property should be populated.
			/// </summary>
			[Display(Description="prefs")]
			Preferencess = 1 << 6,
			/// <summary>
			/// Indicates the Url property should be populated.
			/// </summary>
			[Display(Description="url")]
			Url = 1 << 7,
			/// <summary>
			/// Indicates the Subscribed property should be populated.
			/// </summary>
			[Display(Description="subscribed")]
			Subscribed = 1 << 8,
			/// <summary>
			/// Indicates the LastActivityDate property should be populated.
			/// </summary>
			[Display(Description="dateLastActivity")]
			LastActivityDate = 1 << 9,
			/// <summary>
			/// Indicates the LastViewDate property should be populated.
			/// </summary>
			[Display(Description="dateLastView")]
			LastViewDate = 1 << 10,
			/// <summary>
			/// Indicates the ShortLink property should be populated.
			/// </summary>
			[Display(Description="shortLink")]
			ShortLink = 1 << 11,
			/// <summary>
			/// Indicates the ShortUrl property should be populated.
			/// </summary>
			[Display(Description="shortUrl")]
			ShortUrl = 1 << 12,
		}

		private readonly Field<string> _description;
		private readonly Field<bool?> _isClosed;
		private readonly Field<bool?> _isPinned;
		private readonly Field<bool?> _isStarred;
		private readonly Field<bool?> _isSubscribed;
		private readonly Field<string> _name;
		private readonly Field<Organization> _organization;
		private readonly Field<string> _url;
		private readonly Field<DateTime?> _lastActivity;
		private readonly Field<DateTime?> _lastViewed;
		private readonly Field<string> _shortLink;
		private readonly Field<string> _shortUrl;
		private readonly BoardContext _context;

		private string _id;
		private DateTime? _creation;

		/// <summary>
		/// Specifies which fields should be downloaded.
		/// </summary>
		public static Fields DownloadedFields { get; set; } = (Fields)Enum.GetValues(typeof(Fields)).Cast<int>().Sum();

		/// <summary>
		/// Gets the collection of actions performed on and within the board.
		/// </summary>
		public IReadOnlyCollection<IAction> Actions { get; }
		/// <summary>
		/// Gets the collection of cards contained within the board.
		/// </summary>
		/// <remarks>
		/// This property only exposes unarchived cards.
		/// </remarks>
		public IReadOnlyCollection<ICard> Cards { get; }
		/// <summary>
		/// Gets the creation date of the board.
		/// </summary>
		public DateTime CreationDate
		{
			get
			{
				if (_creation == null)
					_creation = Id.ExtractCreationDate();
				return _creation.Value;
			}
		}
		public IReadOnlyCollection<CustomFieldDefinition> CustomFields { get; }
		/// <summary>
		/// Gets or sets the board's description.
		/// </summary>
		public string Description
		{
			get { return _description.Value; }
			set { _description.Value = value; }
		}
		/// <summary>
		/// Gets the board's ID.
		/// </summary>
		public string Id
		{
			get
			{
				if (!_context.HasValidId)
					_context.Synchronize().Wait();
				return _id;
			}
			private set { _id = value; }
		}
		/// <summary>
		/// Gets or sets whether the board is closed.
		/// </summary>
		public bool? IsClosed
		{
			get { return _isClosed.Value; }
			set { _isClosed.Value = value; }
		}
		/// <summary>
		/// Gets or sets wheterh this board is pinned.
		/// </summary>
		public bool? IsPinned
		{
			get { return _isPinned.Value; }
			set { _isPinned.Value = value; }
		}
		/// <summary>
		/// Gets or sets wheterh this board is pinned.
		/// </summary>
		public bool? IsStarred
		{
			get { return _isStarred.Value; }
			set { _isStarred.Value = value; }
		}
		/// <summary>
		/// Gets or sets whether the current member is subscribed to this board.
		/// </summary>
		public bool? IsSubscribed
		{
			get { return _isSubscribed.Value; }
			set { _isSubscribed.Value = value; }
		}
		/// <summary>
		/// Gets the collection of labels for the board.
		/// </summary>
		public IBoardLabelCollection Labels { get; }
		/// <summary>
		/// Gets the date of the board's most recent activity.
		/// </summary>
		public DateTime? LastActivity => _lastActivity.Value;
		/// <summary>
		/// Gets the date when the board was most recently viewed.
		/// </summary>
		public DateTime? LastViewed => _lastViewed.Value;
		/// <summary>
		/// Gets the collection of lists on this board.
		/// </summary>
		/// <remarks>
		/// This property only exposes unarchived lists.
		/// </remarks>
		public IListCollection Lists { get; }
		/// <summary>
		/// Gets the collection of members on the board.
		/// </summary>
		public IReadOnlyCollection<IMember> Members { get; }
		/// <summary>
		/// Gets the collection of members and their privileges on the board.
		/// </summary>
		public IBoardMembershipCollection Memberships { get; }
		/// <summary>
		/// Gets or sets the board's name.
		/// </summary>
		public string Name
		{
			get { return _name.Value; }
			set { _name.Value = value; }
		}
		/// <summary>
		/// Gets or sets the organization to which the board belongs.
		/// </summary>
		/// <remarks>
		/// Setting null makes the board's first admin the owner.
		/// </remarks>
		public IOrganization Organization
		{
			get { return _organization.Value; }
			set { _organization.Value = (Organization) value; }
		}
		/// <summary>
		/// Gets metadata about any active power-ups.
		/// </summary>
		public IReadOnlyCollection<IPowerUp> PowerUps { get; }
		/// <summary>
		/// Gets specific data regarding power-ups.
		/// </summary>
		public IReadOnlyCollection<IPowerUpData> PowerUpData { get; }
		/// <summary>
		/// Gets the set of preferences for the board.
		/// </summary>
		public IBoardPreferences Preferences { get; }
		/// <summary>
		/// Gets the set of preferences for the board.
		/// </summary>
		public IBoardPersonalPreferences PersonalPreferences { get; }
		/// <summary>
		/// Gets the board's short URI.
		/// </summary>
		public string ShortLink => _shortLink.Value;
		/// <summary>
		/// Gets the board's short link (ID).
		/// </summary>
		public string ShortUrl => _shortUrl.Value;
		/// <summary>
		/// Gets the board's URI.
		/// </summary>
		public string Url => _url.Value;

		/// <summary>
		/// Retrieves a list which matches the supplied key.
		/// </summary>
		/// <param name="key">The key to match.</param>
		/// <returns>The matching list, or null if none found.</returns>
		/// <remarks>
		/// Matches on List.Id and List.Name.  Comparison is case-sensitive.
		/// </remarks>
		public IList this[string key] => Lists[key];
		/// <summary>
		/// Retrieves the list at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The list.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is less than 0 or greater than or equal to the number of elements in the collection.
		/// </exception>
		public IList this[int index] => Lists[index];

		internal IJsonBoard Json
		{
			get { return _context.Data; }
			set { _context.Merge(value); }
		}
		internal TrelloAuthorization Auth { get; }

		/// <summary>
		/// Raised when data on the board is updated.
		/// </summary>
		public event Action<IBoard, IEnumerable<string>> Updated;

		/// <summary>
		/// Creates a new instance of the <see cref="Board"/> object.
		/// </summary>
		/// <param name="id">The board's ID.</param>
		/// <param name="auth">(Optional) Custom authorization parameters. When not provided, <see cref="TrelloAuthorization.Default"/> will be used.</param>
		public Board(string id, TrelloAuthorization auth = null)
		{
			Auth = auth;
			_context = new BoardContext(id, auth);
			_context.Synchronized += Synchronized;
			Id = id;

			Actions = new ReadOnlyActionCollection(typeof(Board), () => Id, auth);
			Cards = new ReadOnlyCardCollection(typeof(Board), () => Id, auth);
			CustomFields = new ReadOnlyCustomFieldDefinitionCollection(() => Id, auth);
			_description = new Field<string>(_context, nameof(Description));
			_isClosed = new Field<bool?>(_context, nameof(IsClosed));
			_isClosed.AddRule(NullableHasValueRule<bool>.Instance);
			_isPinned = new Field<bool?>(_context, nameof(IsPinned));
			_isPinned.AddRule(NullableHasValueRule<bool>.Instance);
			_isStarred = new Field<bool?>(_context, nameof(IsStarred));
			_isStarred.AddRule(NullableHasValueRule<bool>.Instance);
			_isSubscribed = new Field<bool?>(_context, nameof(IsSubscribed));
			_isSubscribed.AddRule(NullableHasValueRule<bool>.Instance);
			Labels = new BoardLabelCollection(() => Id, auth);
			Lists = new ListCollection(() => Id, auth);
			Members = new ReadOnlyMemberCollection(EntityRequestType.Board_Read_Members, () => Id, auth);
			Memberships = new BoardMembershipCollection(() => Id, auth);
			_name = new Field<string>(_context, nameof(Name));
			_name.AddRule(NotNullOrWhiteSpaceRule.Instance);
			_organization = new Field<Organization>(_context, nameof(Organization));
			PowerUps = new ReadOnlyPowerUpCollection(() => Id, auth);
			PowerUpData = new ReadOnlyPowerUpDataCollection(EntityRequestType.Board_Read_PowerUpData, () => Id, auth);
			Preferences = new BoardPreferences(_context.BoardPreferencesContext);
			PersonalPreferences = new BoardPersonalPreferences(() => Id, auth);
			_url = new Field<string>(_context, nameof(Url));
			_shortUrl = new Field<string>(_context, nameof(ShortUrl));
			_shortLink = new Field<string>(_context, nameof(ShortLink));
			_lastActivity = new Field<DateTime?>(_context, nameof(LastActivity));
			_lastViewed = new Field<DateTime?>(_context, nameof(LastViewed));

			TrelloConfiguration.Cache.Add(this);
		}
		internal Board(IJsonBoard json, TrelloAuthorization auth)
			: this(json.Id, auth)
		{
			_context.Merge(json);
		}

		/// <summary>
		/// Applies the changes an action represents.
		/// </summary>
		/// <param name="action">The action.</param>
		public void ApplyAction(IAction action)
		{
			if (action.Type != ActionType.UpdateBoard || action.Data.Board == null || action.Data.Board.Id != Id) return;
			_context.Merge(((Board) action.Data.Board).Json);
		}
		/// <summary>
		/// Permanently deletes the board from Trello.
		/// </summary>
		/// <remarks>
		/// This instance will remain in memory and all properties will remain accessible.
		/// </remarks>
		public async Task Delete()
		{
			await _context.Delete();
			TrelloConfiguration.Cache.Remove(this);
		}
		/// <summary>
		/// Marks the board to be refreshed the next time data is accessed.
		/// </summary>
		public async Task Refresh()
		{
			await _context.Expire();
		}
		/// <summary>
		/// Returns the <see cref="Name"/>.
		/// </summary>
		/// <returns>
		/// A string that represents the attachment.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}

		private void Synchronized(IEnumerable<string> properties)
		{
			Id = _context.Data.Id;
			var handler = Updated;
			handler?.Invoke(this, properties);
		}
	}
}