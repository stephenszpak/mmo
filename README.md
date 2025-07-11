# MMO Client

This repository contains a starting point for a Unity 6.1 MMO client. The project is organized with the usual Unity `Assets` folder and contains stub scripts for combat input, abilities, networking and targeting.  The scene and input actions need to be created inside the Unity Editor.

## Folder Layout

```
Assets/
  Art/            # models, textures
  Animations/
  Audio/
  Prefabs/
  Scenes/
  Scripts/
    Combat/
    Core/
    Input/
    Networking/
    UI/
  Settings/
  UI/
```

## Setup Steps

1. **Create Scene**: Open Unity and create a scene named `Main` in `Assets/Scenes`. Add a flat terrain and a directional light.
2. **Player Controller**: Import the `Starter Assets – Third Person Controller` package and add the prefab to the scene. Replace the capsule with a humanoid rig and ensure a `CharacterController`, `Rigidbody`, `Animator`, and `PlayerInput` component are present.
3. **Camera**: Add a `CinemachineFreeLook` camera in the scene and set its `Follow` target to the player. Attach the `MMOCameraController` script and assign the `Look`, `Orbit`, and `Zoom` actions from your `PlayerControls` asset. Tweak the offsets and speeds in the Inspector.
4. **Input System**: Install the `Input System` package from the Package Manager and enable it in Project Settings. Then create an `Input Actions` asset named `PlayerControls` in `Assets/Settings` with actions `Move`, `Look`, `Jump`, `TargetNext`, `Ability1`‑`Ability5`. Bind them for both keyboard/mouse and gamepad. Hook these callbacks to `CombatInput`.
5. **Scripts**: Attach the provided scripts as needed.

The client is intended to communicate with an Elixir/Phoenix backend using UDP and WebSockets.

