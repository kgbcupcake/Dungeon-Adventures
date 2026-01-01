using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System.Numerics;
using DungeonAdventures.Src.GameData;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DungeonAdventures.Src.GameData.Components;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{
		private bool ValidateStringSelection(string[] sourceArray, int selectedIndex, string fieldName)
		{
			if (selectedIndex < 0 || selectedIndex >= sourceArray.Length)
			{
				_injectionSuccess = false;
				_injectionMsg = $"VALIDATION FAILED: Invalid {fieldName} selection.";
				_showInjectionPopup = true;
				DevLog.Write(_injectionMsg, "ERROR");
				return false;
			}
			return true;
		}

		private void RenderAdvancedWeaponLab()
		{
			ImGui.TextColored(new Vector4(1, 0.4f, 0.4f, 1), "WEAPON FORGE & MODDING BENCH");
			ImGui.Combo("Category", ref _wTypeIndex, _wTypes, _wTypes.Length);
			ImGui.Separator();

			ImGui.InputText("Base Name", ref _wName, 100);
			ImGui.Combo("Rarity Grade", ref _wRarity, _rarities, _rarities.Length);

			ImGui.Columns(2, "weapon_stats", false);
			ImGui.DragInt("Impact Damage", ref _wDamage, 5, 0, 10000);
			ImGui.DragFloat("Carry Weight", ref _wWeight, 0.1f, 0, 100);
			ImGui.NextColumn();
			ImGui.InputInt("Market Value", ref _wPrice);
			ImGui.DragInt("Number of Sockets", ref _wNumSockets, 1, 0, 8);
			ImGui.Columns(1);
			ImGui.DragInt("Critical Chance %", ref _wCritChance, 1, 0, 100);

			string extraStats = "";
			ImGui.Separator();

			if (_wTypeIndex == 0) // MELEE
			{
				ImGui.TextColored(new Vector4(0.4f, 0.8f, 1f, 1), "Melee-Only Attributes");
				ImGui.SliderInt("Structural Integrity", ref _wDurability, 1, 1000);
				extraStats = $" | Integrity: {_wDurability}/{_wDurability}";
			}
			else if ((_wTypeIndex == 1 || _wTypeIndex == 2 || _wTypeIndex == 3) && _wMagSize > 0) // FIREARM/RANGED/HEAVY
			{
				ImGui.TextColored(new Vector4(1f, 0.6f, 0.2f, 1), "Ballistic Specifics");
				ImGui.Combo("Elemental Infusion", ref _wEffectIndex, _wEffects, _wEffects.Length);
				ImGui.DragInt("Magazine Capacity", ref _wMagSize, 1, 1, 500);
				ImGui.Combo("Required Ammo", ref _wAmmoIndex, _ammoTypes, _ammoTypes.Length);

				EffectType currentEffectType = EffectType.None;
				if (Enum.TryParse<EffectType>(_wEffects[_wEffectIndex], true, out var parsedEffect))
				{
					currentEffectType = parsedEffect;
				}
				string effectStr = (currentEffectType != EffectType.None) ? $" [{currentEffectType}]" : "";
				extraStats = $" | Mag: {_wMagSize}{effectStr} | Caliber: {_ammoTypes[_wAmmoIndex]}";
			}
			else if (_wTypeIndex == 4 && _wMagSize > 0) // EXOTIC
			{
				ImGui.TextColored(new Vector4(0.6f, 1f, 0.4f, 1), "Exotic Weapon Properties");
				ImGui.Combo("Elemental Infusion", ref _wEffectIndex, _wEffects, _wEffects.Length);
				ImGui.DragInt("Charge Capacity", ref _wMagSize, 1, 1, 500);
				ImGui.Combo("Energy Type", ref _wAmmoIndex, _ammoTypes, _ammoTypes.Length);

				EffectType currentEffectType = EffectType.None;
				if (Enum.TryParse<EffectType>(_wEffects[_wEffectIndex], true, out var parsedEffect))
				{
					currentEffectType = parsedEffect;
				}
				string effectStr = (currentEffectType != EffectType.None) ? $" [{currentEffectType}]" : "";
				extraStats = $" | Charge: {_wMagSize}{effectStr} | Energy: {_ammoTypes[_wAmmoIndex]}";
			}

			ImGui.Separator();
			ImGui.TextColored(new Vector4(0.5f, 1.0f, 0.5f, 1), "Gem Socketing");

			if (ImGui.Button("Refresh Available Gems"))
			{
				string gemPath = GameState.GetGlobalPath("gems");
				_availableGems = LoadGame.LoadAllFromFolder<GemData>(gemPath);
			}


			if (_wNumSockets > 0)
			{
				string[] availableGemNames = _availableGems.Select(g => g.Name).ToArray();
				if (availableGemNames.Length > 0 && _gemsToSocket.Count < _wNumSockets)
				{
					ImGui.Combo("Select Gem to Socket", ref _selectedGemIndex, availableGemNames, availableGemNames.Length);
					ImGui.SameLine();
					if (ImGui.Button("Add Gem"))
					{
						if (_selectedGemIndex >= 0 && _selectedGemIndex < _availableGems.Count && _gemsToSocket.Count < _wNumSockets)
						{
							_gemsToSocket.Add(_availableGems[_selectedGemIndex]);
						}
					}
				}
				else if (_gemsToSocket.Count >= _wNumSockets)
				{
					ImGui.TextColored(new Vector4(1, 0, 0, 1), "All sockets are full.");
				}

				ImGui.Text($"Socketed Gems ({_gemsToSocket.Count}/{_wNumSockets}):");
				for (int i = 0; i < _gemsToSocket.Count; i++)
				{
					ImGui.Text($"- {_gemsToSocket[i].Name} (Power: {_gemsToSocket[i].Power:F1})");
					ImGui.SameLine();
					if (ImGui.SmallButton($"Remove##Gem{i}"))
					{
						_gemsToSocket.RemoveAt(i);
						break;
					}
				}
			}

			ImGui.Separator();
			ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.4f, 1), "WEAPON ATTACHMENT SOCKETING");

			// 1. Inside the Gem/Socketing section
			if (ImGui.Button("Refresh Available Attachments##Gems"))
			{
				// Point to the 'attachments' subfolder in your Documents
				string path = GameState.GetGlobalPath("attachments");
				_availableAttachments = LoadGame.LoadAllFromFolder<AttachmentData>(path);

				DevLog.Write($"Refreshed Attachments: Found {_availableAttachments.Count} files.", "SYSTEM");
			}

			// 2. Inside the Weapon Attachment section
			if (ImGui.Button("Refresh Available Attachments##Attachments"))
			{
				string path = GameState.GetGlobalPath("attachments");
				_availableAttachments = LoadGame.LoadAllFromFolder<AttachmentData>(path);

				// You could also trigger a sync here if needed
				GameState.Sync();
			}

			foreach (AttachmentSlot slot in Enum.GetValues(typeof(AttachmentSlot)))
			{
				if (slot == AttachmentSlot.None) continue;
				string slotName = slot.ToString();
				var attachmentsForSlot = _availableAttachments.Where(a => a.Slot == slot).ToList();
				AttachmentData existingAttachment = _attachmentsToSocket.FirstOrDefault(a => a.Slot == slot);

				ImGui.PushID($"AttachmentSlot_{slotName}");
				if (existingAttachment == null)
				{
					if (attachmentsForSlot.Any())
					{
						string[] attachmentNames = attachmentsForSlot.Select(a => a.Name).ToArray();
						int currentAttachmentIndex = -1;
						if (ImGui.Combo($"Select {slotName} Attachment", ref currentAttachmentIndex, attachmentNames, attachmentNames.Length))
						{
							if (currentAttachmentIndex >= 0 && currentAttachmentIndex < attachmentsForSlot.Count)
							{
								_attachmentsToSocket.RemoveAll(a => a.Slot == slot);
								_attachmentsToSocket.Add(attachmentsForSlot[currentAttachmentIndex]);
							}
						}
					}
				}
				else
				{
					ImGui.Text($"Socketed {slotName}: {existingAttachment.Name}");
					ImGui.SameLine();
					if (ImGui.Button($"Remove##{slotName}"))
					{
						_attachmentsToSocket.Remove(existingAttachment);
					}
				}
				ImGui.PopID();
			}

			// --- STATS PROJECTION ---
			float totalDmgMod = _attachmentsToSocket.Aggregate(1.0f, (acc, a) => acc * a.DamageMod); float projectedCritChance = _wCritChance;
			float projectedSpeedMod = 1.0f; // For attachment multipliers
			float totalGemDamagePower = 0f;
			float totalGemCritPower = 0f;
			float projectedDamage = _wDamage;

			foreach (var gem in _gemsToSocket)
			{
				foreach (var attribute in gem.Attributes)
				{
					switch (attribute)
					{
						case EffectType.Fire:
						case EffectType.Ice:
						case EffectType.Poison:
						case EffectType.Electric:
							totalGemDamagePower += gem.Power;
							break;
						case EffectType.Speed:
							// Assuming Speed contributes to crit chance
							totalGemCritPower += gem.Power;
							break;
							// Add other gem effects as needed
					}
				}
			}

			// Apply Gem additions
			projectedDamage += totalGemDamagePower;
			projectedCritChance += totalGemCritPower;

			// Apply Attachment multipliers
			foreach (var attachment in _attachmentsToSocket)
			{
				projectedDamage *= attachment.DamageMod;
				projectedCritChance *= attachment.SpeedMod;
			}

			totalDmgMod += totalGemDamagePower;
			projectedCritChance += totalGemCritPower;


			foreach (var attachment in _attachmentsToSocket)
			{
				totalDmgMod *= attachment.DamageMod;
				projectedCritChance *= attachment.SpeedMod; // Assuming speed mod affects crit chance for attachments
				projectedSpeedMod *= attachment.SpeedMod; // Keep a separate speed mod for general display if needed
			}

			ImGui.Separator();
			ImGui.TextColored(new Vector4(0.8f, 0.4f, 0.8f, 1), "PROJECTED STATS:");
			ImGui.Text($"Base Damage: {_wDamage} + Gem Damage: {totalGemDamagePower:F1} = {(_wDamage + totalGemDamagePower):F1}");
			ImGui.Text($"Base Crit Chance: {_wCritChance}% + Gem Crit: {totalGemCritPower:F1}% = {(_wCritChance + totalGemCritPower):F1}%");
			ImGui.Text($"Final Damage: {totalDmgMod:F0} (x{_attachmentsToSocket.Sum(a => a.DamageMod):F1} Attachments)");
			ImGui.Text($"Final Damage: {projectedDamage:F0} (x{totalDmgMod:F2} Multiplier)");
			ImGui.Text($"Final Crit Chance: {projectedCritChance:F0}% (x{_attachmentsToSocket.Sum(a => a.SpeedMod):F1} Attachments)");
			ImGui.Separator();

			// --- FABRICATE BUTTON ---
			if (ImGui.Button("FABRICATE TO DISK", new Vector2(-1, 40)))
			{
				if (string.IsNullOrWhiteSpace(_wName))
				{
					_injectionMsg = "[FAILURE] Weapon Name cannot be empty.";
					_injectionSuccess = false;
					_showInjectionPopup = true;
					return;
				}

				if (Enum.TryParse<DungeonAdventures.Src.GameData.ItemRarity>(_rarities[_wRarity], true, out var parsedRarity) &&
					ValidateStringSelection(_wTypes, _wTypeIndex, "Weapon Type") &&
					Enum.TryParse<EffectType>(_wEffects[_wEffectIndex], true, out var parsedEffect) &&
					ValidateStringSelection(_ammoTypes, _wAmmoIndex, "Ammo Type"))
				{
					WeaponData newWeapon = new WeaponData(
						_wName, _wDamage, _wWeight, _wPrice, parsedRarity,
						_wTypes[_wTypeIndex], parsedEffect, _wDurability,
						_wMagSize, _ammoTypes[_wAmmoIndex], _wCritChance, _wNumSockets
					);
					newWeapon.SocketedGems.AddRange(_gemsToSocket);
					newWeapon.Attachments.AddRange(_attachmentsToSocket);

					try
					{
						string folder = "weapons";
						string fileName = $"{newWeapon.Name.Replace(" ", "_")}.json";
						SaveGame.ExportToMaster(newWeapon, folder, fileName);
						GameState.Sync();

						_gemsToSocket.Clear();
						_attachmentsToSocket.Clear();
						_injectionSuccess = true;
						_injectionMsg = $"FORGE SUCCESSFUL:\nWeapon '{_wName}' fabricated via SaveGame.";
					}
					catch (Exception ex)
					{
						_injectionMsg = $"SAVE FAILED: {ex.Message}";
						_injectionSuccess = false;
						DevLog.Write($"Forge Error: {ex.Message}", "ERROR");
					}
					_showInjectionPopup = true;
				}
				else
				{
					_injectionMsg = "[FAILURE] One or more selections are invalid for Weapon Fabrication.";
					_injectionSuccess = false;
					_showInjectionPopup = true;
				}
			}
		}

		private void RenderItemCreator()
		{
			ImGui.TextColored(new Vector4(0.4f, 1f, 0.4f, 1), "ITEM FABRICATION & ALCHEMY");

			if (ImGui.Combo("Recall Preset", ref _selectedPreset, _itemPresets, _itemPresets.Length))
			{
				switch (_selectedPreset)
				{
					case 1: _iName = "Mega Health Pack"; _iEffectIndex = 0; _iValue = 500; _iTypeIndex = 0; break;
					case 2: _iName = "Infinite Mana"; _iEffectIndex = 1; _iValue = 1000; _iTypeIndex = 0; break;
					case 3: _iName = "Skeleton Key"; _iTypeIndex = 4; _iValue = 0; _iEffectIndex = 9; break;
					case 4: _iName = "God's Lunch"; _iEffectIndex = 2; _iValue = 9999; _iTypeIndex = 0; break;
					case 5: _iName = "Ancient Scroll"; _iTypeIndex = 2; _iValue = 2500; break;
					case 7: _iName = "Potion of Poison"; _iEffectIndex = 10; _iValue = 75; _iTypeIndex = 9; break;
				}
			}

			ImGui.InputText("Item Display Name", ref _iName, 100);
			ImGui.Combo("Classification", ref _iTypeIndex, _itemTypes, _itemTypes.Length);
			ImGui.Combo("Visual Rarity", ref _iRarity, _rarities, _rarities.Length);
			ImGui.Combo("Enchantment Effect", ref _iEffectIndex, _iEffects, _iEffects.Length);

			ImGui.Columns(2, "item_stats_cols", false);
			ImGui.DragInt("Unit Price", ref _iValue, 10, 0, 50000);
			ImGui.DragFloat("Unit Weight", ref _iWeight, 0.05f, 0, 50);
			ImGui.NextColumn();
			ImGui.InputInt("Batch Quantity", ref _iAmount);
			ImGui.Checkbox("Allow Stacking?", ref _isStackable);
			ImGui.Columns(1);

			if (ImGui.Button("FABRICATE TO DISK", new Vector2(-1, 40)))
			{
				if (string.IsNullOrWhiteSpace(_iName))
				{
					_injectionMsg = "[FAILURE] Item Name cannot be empty.";
					_injectionSuccess = false;
					_showInjectionPopup = true;
					return;
				}

				if (Enum.TryParse<ItemRarity>(_rarities[_iRarity], true, out var parsedRarity) && ValidateStringSelection(_itemTypes, _iTypeIndex, "Item Type") &&
					Enum.TryParse<EffectType>(_iEffects[_iEffectIndex], true, out var parsedEffect))
				{
					ItemData newItem = new ItemData(_iName, _iValue, _iWeight, _iAmount, _itemTypes[_iTypeIndex], parsedRarity, parsedEffect, _iDurability, _iCharges, _isStackable, 0);

					try
					{
						string folder = "items";
						string fileName = $"{newItem.Name.Replace(" ", "_")}.json";
						SaveGame.ExportToMaster(newItem, folder, fileName);
						GameState.Sync();

						_injectionSuccess = true;
						_injectionMsg = $"ALCHEMY SUCCESS:\n'{_iName}' fabricated via SaveGame.";
					}
					catch (Exception ex)
					{
						_injectionMsg = $"SAVE FAILED: {ex.Message}";
						_injectionSuccess = false;
						DevLog.Write($"Item Creator Error: {ex.Message}", "ERROR");
					}
					_showInjectionPopup = true;
				}
				else
				{
					_injectionMsg = "[FAILURE] One or more selections are invalid for Item Fabrication.";
					_injectionSuccess = false;
					_showInjectionPopup = true;
				}
			}

			ImGui.SameLine();

			if (ImGui.Button("FABRICATE & INJECT INTO SHOP", new Vector2(-1, 40)))
			{
				if (string.IsNullOrWhiteSpace(_iName))
				{
					_injectionMsg = "[FAILURE] Item Name cannot be empty.";
					_injectionSuccess = false;
					_showInjectionPopup = true;
					return;
				}

				if (Enum.TryParse<ItemRarity>(_rarities[_iRarity], true, out var parsedRarity) && ValidateStringSelection(_itemTypes, _iTypeIndex, "Item Type") &&
					Enum.TryParse<EffectType>(_iEffects[_iEffectIndex], true, out var parsedEffect))
				{
					ItemData newItem = new ItemData(_iName, _iValue, _iWeight, _iAmount, _itemTypes[_iTypeIndex], parsedRarity, parsedEffect, _iDurability, _iCharges, _isStackable, 0);

					try
					{
						GlobalShopManager.Items.Add(newItem); // Add to shop
						string folder = "shop_stock";
						string fileName = $"{newItem.Name.Replace(" ", "_")}.json";
						SaveGame.ExportToMaster(newItem, folder, fileName); // Save to shop_stock folder
						GameState.Sync();

						_injectionSuccess = true;
						_injectionMsg = $"ALCHEMY SUCCESS:\n'{_iName}' fabricated & injected into shop via SaveGame.";
					}
					catch (Exception ex)
					{
						_injectionMsg = $"SAVE FAILED: {ex.Message}";
						_injectionSuccess = false;
						DevLog.Write($"Shop Injection Error: {ex.Message}", "ERROR");
					}
					_showInjectionPopup = true;
				}
				else
				{
					_injectionMsg = "[FAILURE] One or more selections are invalid for Item Fabrication.";
					_injectionSuccess = false;
					_showInjectionPopup = true;
				}
			}
		}

		private void RenderAttachmentCreator()
		{
			ImGui.TextColored(new Vector4(1f, 0.6f, 1f, 1), "WEAPON ATTACHMENT CREATOR");
			ImGui.InputText("Attachment Name", ref _attName, 100);
			ImGui.Combo("Attachment Slot", ref _aSlotIndex, _attSlotIndex, _attSlotIndex.Length);
			ImGui.DragFloat("Damage Multiplier", ref _attDamageMod, 0.01f, 0.0f, 5.0f);
			ImGui.DragFloat("Speed Multiplier", ref _attSpeedMod, 0.01f, 0.0f, 5.0f);

			if (ImGui.Button("EXPORT ATTACHMENT", new Vector2(-1, 40)))
			{
				if (Enum.TryParse<AttachmentSlot>(_attSlotIndex[_aSlotIndex], true, out var parsedSlot) && !string.IsNullOrWhiteSpace(_attName))
				{
					var attachmentData = new AttachmentData { Name = _attName, Slot = parsedSlot, DamageMod = _attDamageMod, SpeedMod = _attSpeedMod };

					// CONDUCTOR FIX: Atomic Save to Documents
					try
					{
						// Replaced Conductor.SaveAsync with SaveGame.ExportToMaster
						string folder = "attachments";
						string fileName = $"{attachmentData.Name.Replace(" ", "_")}.json";
						SaveGame.ExportToMaster(attachmentData, folder, fileName);
						GameState.Sync();

						_injectionSuccess = true;
						_injectionMsg = $"ATTACHMENT CREATED:\n'{_attName}' exported via SaveGame.";
					}
					catch (Exception ex)
					{
						_injectionMsg = $"SAVE FAILED: {ex.Message}";
						_injectionSuccess = false;
						DevLog.Write($"Attachment Creator Error: {ex.Message}", "ERROR");
					}
					_showInjectionPopup = true;
				}
			}
		}

	}
}