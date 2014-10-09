﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ManateeBoardPersonalPreferences.cs
	Namespace:		Manatee.Trello.ManateeJson.Entities
	Class Name:		ManateeBoardPersonalPreferences
	Purpose:		Implements IJsonBoardPersonalPreferences for Manatee.Json.

***************************************************************************************/

using Manatee.Json;
using Manatee.Json.Serialization;
using Manatee.Trello.Json;

namespace Manatee.Trello.ManateeJson.Entities
{
	internal class ManateeBoardPersonalPreferences : IJsonBoardPersonalPreferences, IJsonSerializable
	{
		public bool? ShowSidebar { get; set; }
		public bool? ShowSidebarMembers { get; set; }
		public bool? ShowSidebarBoardActions { get; set; }
		public bool? ShowSidebarActivity { get; set; }
		public bool? ShowListGuide { get; set; }
		public IJsonPosition EmailPosition { get; set; }
		public IJsonList EmailList { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type != JsonValueType.Object) return;
			var obj = json.Object;
			ShowSidebar = obj.TryGetBoolean("showSidebar");
			ShowSidebarMembers = obj.TryGetBoolean("showSidebarMembers");
			ShowSidebarBoardActions = obj.TryGetBoolean("showSidebarBoardActions");
			ShowSidebarActivity = obj.TryGetBoolean("showSidebarActivity");
			ShowListGuide = obj.TryGetBoolean("showListGuide");
			EmailPosition = obj.Deserialize<IJsonPosition>(serializer, "emailPosition");
			EmailList = obj.Deserialize<IJsonList>(serializer, "idEmailList");
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var json = new JsonObject();

			ShowSidebar.Serialize(json, serializer, "showSidebar");
			ShowSidebarMembers.Serialize(json, serializer, "showSidebarMembers");
			ShowSidebarBoardActions.Serialize(json, serializer, "showSidebarBoardActions");
			ShowSidebarActivity.Serialize(json, serializer, "showSidebarActivity");
			ShowListGuide.Serialize(json, serializer, "showListGuide");
			EmailPosition.Serialize(json, serializer, "emailPosition");
			EmailList.Serialize(json, serializer, "idEmailList");

			return json;
		}
	}
}
