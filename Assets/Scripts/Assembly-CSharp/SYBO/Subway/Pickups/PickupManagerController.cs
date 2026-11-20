using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Subway.Pickups.Log;
using SYBO.Subway.Pickups.Picker.Pickers;
using SYBO.Subway.Pickups.Pickers;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class PickupManagerController
	{
		private PickupManager2 _pickupManager;

		private List<Action> _refreshEventSpecificPickupsFunctions = new List<Action>();

		public PickupManagerController(PickupManager2 pickupManager)
		{
			_pickupManager = pickupManager;
			_pickupManager.OnPickerRemoved += _PickupManager_OnPickerRemoved;
			_pickupManager.OnPickupSpawnPointRegistered += _PickupManager_OnPickupSpawnPointRegistered;
			_pickupManager.OnPickupSpawnPointDeregistered += _PickupManager_OnPickupSpawnPointDeregistered;
			_pickupManager.OnPickupSpawned += _PickupManager_OnPickupSpawned;
			_pickupManager.OnPickupCollected += _PickupManager_OnPickupCollected;
			_pickupManager.OnPickupDespawned += _PickupManager_OnPickupDespawned;
			_SetupEventSpecificPickups();
			_RefreshEventSpecificPickups();
		}

		public void Destroy()
		{
			if (_pickupManager != null)
			{
				_pickupManager.OnPickerRemoved -= _PickupManager_OnPickerRemoved;
				_pickupManager.OnPickupSpawnPointRegistered -= _PickupManager_OnPickupSpawnPointRegistered;
				_pickupManager.OnPickupSpawnPointDeregistered -= _PickupManager_OnPickupSpawnPointDeregistered;
				_pickupManager.OnPickupSpawned -= _PickupManager_OnPickupSpawned;
				_pickupManager.OnPickupCollected -= _PickupManager_OnPickupCollected;
				_pickupManager.OnPickupDespawned -= _PickupManager_OnPickupDespawned;
			}
		}

		public void DoUpdate()
		{
			_RefreshEventSpecificPickups();
		}

		private void _SetupEventSpecificPickups()
		{
			_refreshEventSpecificPickupsFunctions = new List<Action>();
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			PickupData2 pickupDataForPickupType2 = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntCharacter);
			PickupData2 pickupDataForPickupType3 = _pickupManager.GetPickupDataForPickupType(PickupType.MysteryPowerup);
			PickupData2 pickupDataForPickupType4 = _pickupManager.GetPickupDataForPickupType(PickupType.SuperMysteryBox);
			PickupData2 pickupDataForPickupType5 = _pickupManager.GetPickupDataForPickupType(PickupType.TokenHunt);
			if (pickupDataForPickupType != null && pickupDataForPickupType.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(_RefreshEventSpecificPickups_LetterHuntDaily);
			}
			if (pickupDataForPickupType2 != null && pickupDataForPickupType2.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(_RefreshEventSpecificPickups_LetterHuntCharacter);
			}
			if (pickupDataForPickupType3 != null && pickupDataForPickupType3.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(_RefreshEventSpecificPickups_MysteryPowerup);
			}
			if (pickupDataForPickupType4 != null && pickupDataForPickupType4.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(_RefreshEventSpecificPickups_SuperMysteryBox);
			}
			if (pickupDataForPickupType5 != null && pickupDataForPickupType5.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(_RefreshEventSpecificPickups_TokenHunt);
			}
		}

		private void _AddAnyEventSpecificPickers()
		{
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			PickupData2 pickupDataForPickupType2 = _pickupManager.GetPickupDataForPickupType(PickupType.TokenHunt);
			PickupData2 pickupDataForPickupType3 = _pickupManager.GetPickupDataForPickupType(PickupType.SuperMysteryBox);
			PickupData2 pickupDataForPickupType4 = _pickupManager.GetPickupDataForPickupType(PickupType.MysteryPowerup);
			PickerData2 pickerDataForPickerType = _pickupManager.GetPickerDataForPickerType(PickerType.LetterHuntDaily);
			PickerData2 pickerDataForPickerType2 = _pickupManager.GetPickerDataForPickerType(PickerType.TokenHunt);
			PickerData2 pickerDataForPickerType3 = _pickupManager.GetPickerDataForPickerType(PickerType.SuperMysteryBox);
			PickerData2 pickerDataForPickerType4 = _pickupManager.GetPickerDataForPickerType(PickerType.MysteryPowerUp);
			if (pickupDataForPickupType != null && pickupDataForPickupType.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<LetterHuntDailyPicker>() && pickerDataForPickerType.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new LetterHuntDailyPicker());
			}
			if (pickupDataForPickupType2 != null && pickupDataForPickupType2.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<TokenHuntPicker>() && pickerDataForPickerType2.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new TokenHuntPicker());
			}
			if (pickupDataForPickupType3 != null && pickupDataForPickupType3.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<SuperMysteryBoxPicker>() && pickerDataForPickerType3.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new SuperMysteryBoxPicker());
			}
			if (pickupDataForPickupType4 != null && pickupDataForPickupType4.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<MysteryPowerupPicker>() && pickerDataForPickerType4.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new MysteryPowerupPicker());
			}
		}

		private void _RefreshEventSpecificPickups()
		{
			_AddAnyEventSpecificPickers();
			foreach (Action refreshEventSpecificPickupsFunction in _refreshEventSpecificPickupsFunctions)
			{
				refreshEventSpecificPickupsFunction();
			}
		}

		private void _RefreshEventSpecificPickups_SuperMysteryBox()
		{
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.SuperMysteryBox);
			if (pickupDataForPickupType != null)
			{
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SuperMysteryBox);
				if (activeEventManager != null || Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___SUPER_MYSTERY_BOX)
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void _RefreshEventSpecificPickups_TokenHunt()
		{
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.TokenHunt);
			if (pickupDataForPickupType != null)
			{
				if (WeeklyHuntManager.IsAnyPeriodActive() || Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER)
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void _RefreshEventSpecificPickups_MysteryPowerup()
		{
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.MysteryPowerup);
			if (pickupDataForPickupType != null)
			{
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.MysteryPowerup);
				if (activeEventManager != null || Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___MYSTERY_POWER_UP)
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void _RefreshEventSpecificPickups_LetterHuntDaily()
		{
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			bool flag = false;
			if (pickupDataForPickupType != null && (PlayerInfo.Instance.IsActiveDailyLetterEvent() || Globals.DEBUG_USE_ENABLE_PLAYER_INFO___IS_ACTIVE_DAILY_LETTER_EVENT) && (PlayerInfo.Instance.HasDailyWordTimeLeft() || Globals.DEBUG_USE_ENABLE_PLAYER_INFO___HAS_DAILY_WORD_TIME_LEFT))
			{
				flag = true;
			}
			if (flag)
			{
				pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
			}
			else
			{
				pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
			}
		}

		private bool _IsCharacterNameEventActive()
		{
			return SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName) != null;
		}

		private bool _IsCharacterLetterPickupSpawnable()
		{
			bool result = false;
			if (Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER)
			{
				return true;
			}
			if (_IsCharacterNameEventActive())
			{
				CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
				if (typeOfActiveEventManager != null)
				{
					result = typeOfActiveEventManager.ShouldSpawnLetters();
				}
			}
			return result;
		}

		private void _RefreshEventSpecificPickups_LetterHuntCharacter()
		{
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntCharacter);
			if (Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER)
			{
				pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
			}
			else if (pickupDataForPickupType != null)
			{
				if (_IsCharacterNameEventActive() && _IsCharacterLetterPickupSpawnable())
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void _PickupManager_OnPickupSpawnPointRegistered(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.SPAWN_POINT_REGISTER, pickupSpawnPoint.SpawnPointDifficulty, pickupSpawnPoint.SpawnPointMode));
			}
		}

		private void _PickupManager_OnPickupSpawnPointDeregistered(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.SPAWN_POINT_DEREGISTER, pickupSpawnPoint.PickupData, pickupSpawnPoint.SpawnPointMode));
			}
		}

		private void _PickupManager_OnPickupSpawned(PickupData2 pickupData, SpawnPointMode spawnPointMode, PickupSpawnPoint2 spawnPoint)
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.PICKUP_SPAWN, pickupData, spawnPointMode));
			}
		}

		private void _PickupManager_OnPickupCollected(PickupData2 pickupData, SpawnPointMode spawnPointMode, PickupSpawnPoint2 spawnPoint)
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.PICKUP_COLLECT, pickupData, spawnPointMode));
			}
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && SingletonMonobehavior<PickupManager2>.Instance.WillRewardUponPickupCollected)
			{
				switch (pickupData.PickupType)
				{
				case PickupType.CoinMagnet:
					CoinMagnetPickup.DoReward();
					break;
				case PickupType.JetPack:
					JetpackPickup.DoReward();
					break;
				case PickupType.Key:
					SaveMeTokenPickup.DoReward();
					break;
				case PickupType.LetterHuntDaily:
					DailyLetterPickup.DoReward();
					break;
				case PickupType.LetterHuntCharacter:
				{
					CharacterLetterPickup component = spawnPoint.SpawnedPickupIcon.GetComponent<CharacterLetterPickup>();
					if (component != null)
					{
						component.DoReward();
					}
					break;
				}
				case PickupType.MultiplierScore2x:
					DoubleScoreMultiplierPickup.DoReward();
					break;
				case PickupType.MysteryBox:
					MysteryBoxPickup.DoReward();
					break;
				case PickupType.MysteryPowerup:
					MysteryPowerupPickup.DoReward();
					break;
				case PickupType.PogoStick:
					PogoStickPickup.DoReward();
					break;
				case PickupType.SuperMysteryBox:
					SuperMysteryBoxPickup.DoReward();
					break;
				case PickupType.SuperSneakers:
					SuperSneakersPickup.DoReward();
					break;
				case PickupType.TokenHunt:
					PickupHuntToken.DoReward();
					break;
				default:
					throw new UnityException("No Reward Available for PickupType of " + pickupData.PickupType);
				}
			}
			else
			{
				Debug.Log("Game._OnPickupCollected() Reward Disabled:  " + pickupData.PickupType);
			}
		}

		private void _PickupManager_OnPickupDespawned(PickupData2 pickupData, SpawnPointMode spawnPointMode, PickupSpawnPoint2 spawnPoint)
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.PICKUP_DESPAWN, pickupData, spawnPointMode));
			}
		}

		private void _PickupManager_OnPickerRemoved(PickerRemoveRequestEvent pickerRemoveEvent)
		{
			IPicker iPicker = pickerRemoveEvent.IPicker;
			Type type = iPicker.GetType();
			Debug.Log("\t_PickerCollection_OnPickerRequestRemoval: " + type);
			if (type == typeof(XTestPicker))
			{
				if (_pickupManager._pickerCollection.HasPickerOfType<DefaultPicker>())
				{
				}
				return;
			}
			throw new UnityException("Type not yet expected. Add a case for " + type);
		}
	}
}
