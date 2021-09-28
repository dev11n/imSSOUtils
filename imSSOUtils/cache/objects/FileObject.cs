using System.Drawing;
using System.Globalization;
using System.Numerics;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using imSSOUtils.cores;
using imSSOUtils.window.windows;

namespace imSSOUtils.cache.objects
{
    /// <summary>
    /// Rough File Object access
    /// </summary>
    internal readonly struct FileObject
    {
        /// <summary>
        /// Tries to get the file object name from an object.
        /// </summary>
        /// <param name="gObject">The object which file object's name we want</param>
        /// <returns></returns>
        public static string get_name(string gObject)
        {
            CVar.write_cvar01($"{gObject}::GetFileObjectName()", "String");
            var obj = CVar.read_cvar01_string() ?? "This object isn't apart of a FileObject!";
            return obj;
        }

        /// <summary>
        /// Spawns a single file object instance, overriding all previous instances.
        /// </summary>
        /// <param name="foName">The file object identifier</param>
        /// <param name="gMove">The object this file object should be teleported to</param>
        public static void spawn_file_object(string foName, string gMove) => MemoryAdapter.direct_call(
            "Game->Silverglade->Gameplay_E01->QC40->Crane::FileObjectUnLoad();\n" +
            $"Game->Silverglade->Gameplay_E01->QC40->Crane::SetFileObjectName(\"{foName}\");\n" +
            "Game->Silverglade->Gameplay_E01->QC40->Crane::FileObjectLoad();\n" +
            $"Game->Silverglade->Gameplay_E01->QC40->Crane::Move({gMove});");

        /// <summary>
        /// Initializes the spawner.
        /// </summary>
        public static Vector3 initialize_spawner()
        {
            MemoryAdapter.direct_call(
                "Game->PreShadowWitches::FileObjectUnLoad();\n" +
                "Game->PreShadowWitches::SetFileObjectName(\"FO_GHorse62_Keypose\");\n" +
                "Game->PreShadowWitches::FileObjectLoad();\n" +
                "Game->CableWayExcavator::FileObjectUnLoad();\n" +
                "Game->CableWayExcavator::SetFileObjectName(\"FO_GHorse62_Keypose\");\n" +
                "Game->CableWayExcavator::FileObjectLoad();\n" +
                "Game->CableWayExcavator::Move(CurrentHorse);\n" +
                "Game->PreShadowWitches::Move(Game->CableWayExcavator);");
            var currentPos = PXInternal.get_horse_position();
            ConsoleWindow.send_input($"began at {currentPos.X}, {currentPos.Y}, {currentPos.Z}", "[spawner]",
                Color.White);
            return currentPos;
        }

        /// <summary>
        /// Deactivates the spawner.
        /// </summary>
        public static void deactivate_spawner() => MemoryAdapter.direct_call(
            "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::FileObjectUnLoad();\n" +
            "Game->CableWayExcavator::FileObjectUnLoad();");

        /// <summary>
        /// Spawns a file object which has no instance limit unlike <see cref="spawn_file_object(string,string)"/>.
        /// <para>This is unsafe and may crash the game</para>
        /// </summary>
        /// <param name="foName">The file object identifier</param>
        public static Vector3 spawn_file_object(string foName)
        {
            // ! Cache the horse pos before the code is executed, otherwise it might get launched into the air due to collision and give the wrong result
            var position = PXInternal.get_horse_position();
            MemoryAdapter.direct_call(
                "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::FileObjectUnLoad();\n" +
                $"Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::SetFileObjectName(\"{foName}\");\n" +
                "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::FileObjectLoad();");
            MemoryAdapter.direct_call("Game->PreShadowWitches->Pelvis::SetSkinMeshAnimationSpeed(0);\n" +
                                      "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::Move(CurrentHorse);\n" +
                                      "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::SetRotationZ(180);\n" +
                                      "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::AdjustAndAlignToGround();" +
                                      "Game->PreShadowWitches->Pelvis::Copy(Game->CableWayExcavator->Pelvis);\n" +
                                      "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::Move(Game->PreShadowWitches);");
            return position;
        }

        /// <summary>
        /// Spawns a file object which has no instance limit unlike <see cref="spawn_file_object(string,string)"/>.
        /// <para>This is unsafe and may crash the game</para>
        /// </summary>
        /// <param name="foName">The file object identifier</param>
        /// <param name="scale">The scale of the file object</param>
        /// <param name="differenceY"></param>
        public static Vector3 spawn_file_object(string foName, Vector3 scale, float differenceY)
        {
            var culture = CultureInfo.InvariantCulture;
            const string find = ".", replace = "::";
            // ! Cache the horse pos before the code is executed, otherwise it might get launched into the air due to collision and give the wrong result
            var position = PXInternal.get_horse_position();
            string alpineX = scale.X.ToString(culture).Replace(find, replace),
                alpineY = scale.Y.ToString(culture).Replace(find, replace),
                alpineZ = scale.Z.ToString(culture).Replace(find, replace),
                addY = (position.Y - differenceY).ToString(culture).Replace(find, replace);
            MemoryAdapter.direct_call(
                "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::FileObjectUnLoad();\n" +
                $"Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::SetFileObjectName(\"{foName}\");\n" +
                "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::FileObjectLoad();");
            MemoryAdapter.direct_call("Game->PreShadowWitches->Pelvis::SetSkinMeshAnimationSpeed(0);\n" +
                                      "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::Move(CurrentHorse);\n" +
                                      "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::SetRotationZ(180);\n" +
                                      $"Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::AddPositionY({addY});\n" +
                                      $"Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::SetScale({alpineX}, {alpineY}, {alpineZ});");
            MemoryAdapter.direct_call("Game->PreShadowWitches->Pelvis::Copy(Game->CableWayExcavator->Pelvis);\n" +
                                      "Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::Move(Game->PreShadowWitches);");
            return position;
        }

        // ! Not done and might not even be added.
        public static void set_real_object(string foName, Vector3 position, Vector3 rotation)
        {
            var culture = CultureInfo.InvariantCulture;
            const string find = ".", replace = "::";
            string alpineX = position.X.ToString(culture).Replace(find, replace),
                alpineY = position.Y.ToString(culture).Replace(find, replace),
                alpineZ = position.Z.ToString(culture).Replace(find, replace);
            MemoryAdapter.direct_call(
                "Game->Silverglade->Gameplay_E01->QC40->Crane->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::FileObjectUnLoad();\n" +
                $"Game->Silverglade->Gameplay_E01->QC40->Crane->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::SetFileObjectName(\"{foName}\");\n" +
                "Game->Silverglade->Gameplay_E01->QC40->Crane->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::FileObjectLoad();");
            MemoryAdapter.direct_call(
                "Game->Silverglade->Gameplay_E01->QC40->Crane->Pelvis::SetSkinMeshAnimationSpeed(0);\n" +
                "Game->Silverglade->Gameplay_E01->QC40->Crane->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::Move(Game->PreShadowWitches->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse);\n" +
                "Game->Silverglade->Gameplay_E01->QC40->Crane->Pelvis->Bip001_Pelvis->Spine->Spine1->BackOnHorse::SetPosition(0, 1::2, 0);\n" +
                $"Game->Silverglade->Gameplay_E01->QC40->Crane::SetRotation{rotation.X}, {rotation.Y}, {rotation.Z});\n" +
                $"Game->Silverglade->Gameplay_E01->QC40->Crane::SetPosition({alpineX}, {alpineY}, {alpineZ});");
        }
    }
}