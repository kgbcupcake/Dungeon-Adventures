using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Entities;
using System.Numerics;
using System.Runtime.InteropServices;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{
		#region Variables and State Management
		#region//Attachment Factory Variables
		// Attachment Factory Variables
		private string _attName = "New Attachment";
		public int _aSlotIndex = 0; // THE NUMBER
		public string[] _attSlotIndex = Enum.GetNames(typeof(AttachmentSlot)); // THE ARRAY
		public float _attDamageMod = 1.0f;
		public float _attSpeedMod = 1.0f;
		private string[] _attSlotTypes = new string[] { "Optics", "Barrel", "Underbarrel", "Magazine" };
		private List<AttachmentData> _availableAttachments = new List<AttachmentData>();
		private List<AttachmentData> _attachmentsToSocket = new List<AttachmentData>();
		#endregion
		#region//Weapon Factory
		// Weapon Factory Variables
		public string _wName = "God-Killer";
		public int _wDamage = 999;
		public float _wWeight = 0.5f;
		public int _wPrice = 5000;
		public int _wRarity = 0;
		public bool _godMode = false;
		public string[] _rarities = { "Common", "Rare", "Exotic", "Artifact", "Legendary", "Ethereal" };
		public int _wTypeIndex = 0;
		public string[] _wTypes = { "Melee", "Firearm", "Ranged", "Heavy", "Exotic" };
		public int _wEffectIndex = 0;
		public string[] _wEffects = Enum.GetNames(typeof(EffectType));
		public int _wDurability = 100;
		public int _wMagSize = 30;
		public int _wAmmoIndex = 0;
		public int _wCritChance = 5; // NEW
		public int _wNumSockets = 0;
		private List<GemData> _gemsToSocket = new List<GemData>();
		public string[] _ammoTypes = {
					"9mm",
					".45 ACP",
					"12-Gauge",
					"7.62mm",
					".50 BMG",
					"Energy Cell",
					"Bolt",
					"Arrow",
					"Plasma Cartridge",
					".357 Magnum"
				};
		#endregion
		#region//Item Factory Variables
		// Item Factory Variables
		public string _iName = "Mysterious Potion";
		public int _iValue = 100;
		public float _iWeight = 0.1f;
		public int _iAmount = 1;
		public int _iTypeIndex = 0;
		public string[] _itemTypes = {
			"Consumable",
			"Quest Item",
			"Scroll",
			"Material",
			"Key",
			"Misc",
			"Tool",
			"Armor",
			"Accessory",
			"Potion", // NEW
			"Gem", // NEW
			"Rune", // NEW
			"Relic" // NEW
		};
		public int _iRarity = 0;
		public int _iEffectIndex = 0;
		public string[] _iEffects = Enum.GetNames(typeof(EffectType));
		public int _iDurability = 100;
		public int _iCharges = 5;
		public bool _isStackable = true;
		public int _selectedPreset = 0;
		public string[] _itemPresets = 
		{
					"Custom",
					"Mega Health Pack",
					"Infinite Mana",
					"Skeleton Key",
					"God's Lunch",
					"Ancient Scroll",
					"Broken Compass",
					"Potion of Poison", // NEW
		            "Gem of Freezing", // NEW
		            "Rune of Burning", // NEW
		            "Relic of Confusion" // NEW
		 };
		#endregion
		#region// Window & Engine State
		public bool _isFirstFrame = true;
		public DateTime _lastToggleTime = DateTime.MinValue;
		public bool _devModeInternal = false;
		public bool _logPaused = false;
		public string _logFilter = "";
		public string _profileSearchFilter = "";
		public bool _isDeleteOpen = true; // Required by ImGui for modal popups
		public string GameVersion { get; set; }
		public string DevName { get; set; }

		[DllImport("user32.dll")]
		static extern short GetAsyncKeyState(int key);
		public bool IsVisible { get; set; } = false;
		// FIX: Add this to prevent the "F8" toggle from flickering too fast
		private DateTime _lastF8ToggleTime = DateTime.Now;
		#endregion
		#region//File & Profile
		// File & Profile Management System
		public bool _showDeleteConfirm = false;
		public bool _showEditPopup = false;
		public bool _showCreatePopup = false;
		public string _profileToDelete = "";
		public string _profileToEdit = "";
		public string _jsonEditorBuffer;
		public string _currentLoadedFileName = "None";
		public string _newProfileName = "NewHero";
		public string _lastSavedTime = "Never";
		public string _pNameBuffer = "New Hero";
		public loadPlayer _tempNewPlayer = new loadPlayer();
		public static bool NeedsMenuRedraw = false; //
		private string _fileToDelete = string.Empty;
		public List<WeaponData> _availableWeapons = new(); // Fixes '_availableWeapons' errors
		#endregion
		#region// Notification & Injection System
		public static string _injectionMsg = "";
		public static bool _injectionSuccess = false;
		public static bool _showInjectionPopup = false;
		#endregion
		#region//Character & Class
		// Character & Class Variables
		private int _selectedClassIndex = 0;
		private string[] _availableClasses = {
					"Warrior",
					"Mage",
					"Rogue",
					"Paladin",
					"Necromancer",
					"Berserker",
					"Ranger",
					"Cleric",
					"Bard"
				};
		#endregion
		#region//Text RPG Engine
		// Text RPG Engine Testing Variables
		public string _testDialogue = "The old man looks at {PlayerName} and says, '[Red]Beware the shadows![White]'";
		public string _testRoomDesc = "You stand in a cold, damp cell. The stone walls are covered in moss, and a single rusted chain hangs from the ceiling.";
		public Vector3 _warpCoords = new Vector3(0, 0, 0);
		public string _targetRoomID = "Cellar_01";
		public bool _showWarpMenu = false;
		#endregion
		#region//Customization
		//Customization - REMOVED due to CS0414 warning
		//private bool Customization = false;
		//private bool MainGui = true;
		public Vector4 color1 = new Vector4(1, 1, 1, 1);       // Font
		public Vector4 BorderColor = new Vector4(0, 1, 1, 1);  // Border
		public float sd1 = 1.0f; // Window Border Size
		public float sd2 = 1.0f; // Frame Border Size Child
		public float sd4 = 1.0f; // Child Border Size
		public float sd5 = 5.0f; // Window Rounding
		public float sd6 = 4.0f; // Frame Rounding
		public float sd7 = 4.0f; // Child Rounding
		public Vector2 sd3 = new Vector2(900, 700); // Window Size
		public Vector4 TgbColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f); // 1.0f is fully opaque
		public Vector4 ChildBgColor = new Vector4(0.08f, 0.08f, 0.12f, 1.0f);
		#endregion
		#region Adventure Creator Variables
		private string _aMapName = "New Adventure";
		private string _aScenePath = "Assets/Scenes/NewAdventure.unity";
		private int _aDifficulty = 0; // Index for Difficulty enum
		private string[] _difficulties = { "Easy", "Medium", "Hard", "Very Hard", "Legendary" };
		private int _aRecommendedLevel = 1;
		private List<string> _aTags = new List<string>();
		private string _newAdventureTag = ""; // For adding new tags
		#endregion
		#region//Quest Designer State
		private string _qTitle = "New Quest";
		private string _qObjective = "";
		private int _qGoldReward = 0;
		private int _qXPReward = 0;
		#endregion
		#region//Dungeon Builder State
		private string _dName = "New Dungeon";
		private int _dFloors = 1;
		private bool _dIsHardcore = false;
		#endregion
		#region Boss Creator Variables
		public string _bossName = "New Boss";
		public float _bossHP = 1000f;
		public float _bossSpeed = 10f;
		public int _bossLevel = 1;
		public float _bossDamage = 50f;
		public string _newBossAbility = ""; // Fixed the missing ability string
		public List<TraitType> _bAbilities = new();
		public List<TraitType> _bossTraits = new List<TraitType>();
		public int _bossAuraIndex = 0; // Persistent index for the Aura dropdown
		#endregion
		#region//Gem Alchemy Variables
		public string _gName = "New Shard";
		public int _gRarity = 0;
		public float _gPower = 10.0f;
		public int _gValue = 100;      // New
		public float _gWeight = 0.1f;  // New
		public int _gLevelReq = 1;     // New
		public List<EffectType> _gAttributes = new();

		public string[] _rarityNames = Enum.GetNames(typeof(ItemRarity));
		public EffectType[] _allEffects = (EffectType[])Enum.GetValues(typeof(EffectType));
		private float _pulseTimer = 0.0f;

		public string[] _gemEffects =
		{
		"Bonus HP", "Bonus MP", "Bonus STR", "Bonus DEX",
		"Bonus INT", "Fire Damage", "Ice Damage", "Poison Damage"
		};
		public int _selectedGemIndex = 0;
		public List<GemData> _availableGems = new List<GemData>();
		#endregion

		#endregion
























	}
}
