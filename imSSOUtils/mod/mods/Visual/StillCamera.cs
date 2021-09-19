namespace imSSOUtils.mod.mods.Visual
{
    /// <summary>
    /// Locks the camera into place.
    /// </summary>
    internal class StillCamera : IMod
    {
        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger() => alpine_execute(
            "Game->CutSceneCamera::Move(CurrentHorse->Skin->Pelvis->Bip001_Pelvis->Spine->Spine1->Spine2->SaddleJoint);\nGame->CutSceneCamera->LookAt::SetLookAtTarget(CurrentPlayer);\nGame->CutSceneCamera::SetRotationZ(0);\nGame->CutSceneCamera::Start();");

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() =>
            add_button("Reset_Camera", "Game->PlayerCamera::Start();");
    }
}