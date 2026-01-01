using NUnit.Framework;
using DungeonAdventures.Src.GameData.Entities;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;
using DungeonAdventures.Src.GameEngine; // Added for SaveGame and LoadGame
using System.IO; // Added for Path and Directory
using DungeonAdventures.Src.GameData.Components; // Added for GameState

namespace DungeonAdventures.Tests
{
    public class PlayerTests
    {
        private string _testProfilesPath;

        [SetUp]
        public void Setup()
        {
            // Create a unique temporary directory for each test fixture
            _testProfilesPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_testProfilesPath);
            // Temporarily redirect GameState.MasterPath for testing purposes
            // This is a simplification; in a real project, consider dependency injection for better test isolation.
            GameState.MasterPath = Path.GetDirectoryName(_testProfilesPath);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the temporary directory
            if (Directory.Exists(_testProfilesPath))
            {
                Directory.Delete(_testProfilesPath, true);
            }
        }

        [Test]
        public void LoadPlayer_WeaponValue_ShouldBeSettableAndGettable()
        {
            // Arrange
            loadPlayer player = new loadPlayer();
            int expectedWeaponValue = 15;

            // Act
            player.WeaponValue = expectedWeaponValue;
            int actualWeaponValue = player.WeaponValue;

            // Assert
            Assert.AreEqual(expectedWeaponValue, actualWeaponValue);
        }

        [Test]
        public void LoadPlayer_ArmorValue_ShouldBeSettableAndGettable()
        {
            // Arrange
            loadPlayer player = new loadPlayer();
            int expectedArmorValue = 10;

            // Act
            player.ArmorValue = expectedArmorValue;
            int actualArmorValue = player.ArmorValue;

            // Assert
            Assert.AreEqual(expectedArmorValue, actualArmorValue);
        }

        [Test]
        public void LoadPlayer_WeaponAndArmorValue_ShouldBeSerializedAndDeserializedCorrectly()
        {
            // Arrange
            loadPlayer originalPlayer = new loadPlayer
            {
                PlayerName = "TestPlayer",
                WeaponValue = 25,
                ArmorValue = 20
            };
            string fileName = "TestPlayerSave";
            string folder = Path.GetFileName(_testProfilesPath); // Use the name of the temp folder as subfolder

            // Act - Save the player data
            SaveGame.ExportToMaster(originalPlayer, folder, fileName);

            // Construct the full file path for loading
            string savedFilePath = Path.Combine(_testProfilesPath, fileName + ".json");

            // Assert that the file was created
            Assert.IsTrue(File.Exists(savedFilePath), "Save file was not created.");

            // Act - Load the player data
            loadPlayer? loadedPlayer = LoadGame.LoadProfile(savedFilePath);

            // Assert
            Assert.NotNull(loadedPlayer, "Loaded player should not be null.");
            Assert.AreEqual(originalPlayer.PlayerName, loadedPlayer.PlayerName, "PlayerName mismatch.");
            Assert.AreEqual(originalPlayer.WeaponValue, loadedPlayer.WeaponValue, "WeaponValue mismatch.");
            Assert.AreEqual(originalPlayer.ArmorValue, loadedPlayer.ArmorValue, "ArmorValue mismatch.");
        }
    }
}