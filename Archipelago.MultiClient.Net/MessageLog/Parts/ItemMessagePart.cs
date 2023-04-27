﻿using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;

namespace Archipelago.MultiClient.Net.MessageLog.Parts
{
	public class ItemMessagePart : MessagePart
	{
		public ItemFlags Flags { get; }
		public long ItemId { get; }

		internal ItemMessagePart(IReceivedItemsHelper items, JsonMessagePart part) : base(MessagePartType.Item, part)
		{
			Flags = part.Flags ?? ItemFlags.None;
			Color = GetColor(Flags);

			switch (part.Type)
			{
				case JsonMessagePartType.ItemId:
					ItemId = long.Parse(part.Text);
					Text = items.GetItemName(ItemId) ?? $"Item: {ItemId}";
					break; 
				case JsonMessagePartType.ItemName:
					ItemId = 0; // we are not going to try to reverse lookup this value based on the game of the receiving player
					Text = part.Text;
					break;
			}
		}

		static Color GetColor(ItemFlags flags)
		{
			if (HasFlag(flags, ItemFlags.Advancement))
				return Color.Plum;
			if (HasFlag(flags, ItemFlags.NeverExclude))
				return Color.SlateBlue;
			if (HasFlag(flags, ItemFlags.Trap))
				return Color.Salmon;

			return Color.Cyan;
		}

		static bool HasFlag(ItemFlags flags, ItemFlags flag) =>
#if NET35
			(flags & flag) > 0;
#else
            flags.HasFlag(flag);
#endif
	}
}