using System.Collections.Generic;
using System.Numerics;
using imSSOUtils.adapters;
using imSSOUtils.cache.objects;
using imSSOUtils.window.windows;

namespace imSSOUtils.cores
{
    internal struct PXFileObject
    {
        /// <summary>
        /// The objects FO_ Identifier
        /// </summary>
        public readonly string identifier;

        /// <summary>
        /// The world-space position of this object
        /// </summary>
        public Vector3 worldSpacePosition;

        /// <summary>
        /// The scale of this object
        /// </summary>
        public Vector3 scale;

        public PXFileObject(string identifier, Vector3 worldSpacePosition, Vector3 scale)
        {
            this.identifier = identifier;
            this.worldSpacePosition = worldSpacePosition;
            this.scale = scale;
        }
    }

    /// <summary>
    /// The core of Object Spawner.
    /// </summary>
    internal readonly struct SpawnerCore
    {
        #region Variables
        /// <summary>
        /// All current objects.
        /// </summary>
        public static readonly List<PXFileObject> objects = new();

        //public static string closestObject = string.Empty;

        /// <summary>
        /// The spawner position.
        /// </summary>
        public static Vector3 spawnerPosition;

        //public static Timer distanceCheck;

        /// <summary>
        /// Determines whether or not the Spawner has been created.
        /// </summary>
        public static bool isInitialized;
        #endregion

        /// <summary>
        /// Gets the amount of objects currently spawned.
        /// </summary>
        /// <returns></returns>
        public static int get_objects_count() => objects.Count;

        /// <summary>
        /// Deactivates the spawner and optionally clears the objects.
        /// </summary>
        public static void deactivate(bool clearObjects)
        {
            if (clearObjects) objects.Clear();
            FileObject.deactivate_spawner();
            toggle_distance_checking();
            isInitialized = false;
        }

        /// <summary>
        /// Activates the spawner.
        /// </summary>
        public static void activate()
        {
            // The spawner will be placed at a different position when executed again, so all objects will become offset.
            // Therefore we just simply reset them.
            objects.Clear();
            spawnerPosition = FileObject.initialize_spawner();
            toggle_distance_checking();
            isInitialized = true;
        }

        /// <summary>
        /// Spawns a new file object.
        /// </summary>
        /// <param name="foIdentifier"></param>
        /// <param name="scale"></param>
        public static void spawn(string foIdentifier, Vector3 scale)
        {
            if (!isInitialized) return;
            var @object = new PXFileObject(foIdentifier,
                FileObject.spawn_file_object(foIdentifier, scale, spawnerPosition.Y), scale);
            objects.Add(@object);
            ExtraInfoWindow.log(
                $"[debug] SPAWNER >> SPAWNED {foIdentifier} AT {@object.worldSpacePosition.X}, {@object.worldSpacePosition.Y}, {@object.worldSpacePosition.Z}");
        }

        /// <summary>
        /// Load all objects from a specific save file.
        /// </summary>
        /// <param name="name"></param>
        public static void load(string name)
        {
            // ! Stop if it was enabled prior
            if (isInitialized) return;
            FileAdapter.load_object_preset(name);
            // ! Enable Object Spawner on our own
            FileObject.initialize_spawner();
            var lastPos = PXInternal.get_horse_position();
            for (var i = 0; i < objects.Count; i++)
            {
                var current = objects[i];
                var currentIdentifier = current.identifier;
                Vector3 currentPos = current.worldSpacePosition, currentScale = current.scale;
                PXInternal.set_horse_position(currentPos);
                FileObject.spawn_file_object(currentIdentifier, currentScale, spawnerPosition.Y);
                ExtraInfoWindow.log($"[debug] LOADED OBJECT {currentIdentifier} FROM {name}");
            }

            PXInternal.set_horse_position(lastPos);
            isInitialized = true;
        }

        public static void toggle_distance_checking()
        {
            /*
            if (distanceCheck is not null)
            {
                distanceCheck.Dispose();
                return;
            }

            distanceCheck = new Timer(_ =>
            {
                for (var i = 0; i < objects.Count; i++)
                {
                    var world = objects[i].worldSpacePosition;
                    var horse = PXInternal.get_horse_position();
                    var distance = Vector3.Distance(horse, world);
                    // TODO: Fix rotation
                    closestObject =
                        $"closest seems to be {objects[i].identifier} within {distance}, rotation Y: {objects[i].rotation.Y}";
                    FileObject.set_real_object(objects[i].identifier, world, objects[i].rotation);
                }
            }, null, 0, 500);
            */
        }
    }
}