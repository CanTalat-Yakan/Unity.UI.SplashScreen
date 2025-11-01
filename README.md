# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# UI Toolkit Splash Screen

> Quick overview: Three‑act, UI Toolkit‑driven splash sequence with timed material/light/lens‑flare animation, synced audio, and easy branding via textures and text. Ships with a one‑click prefab spawner.

A drop‑in splash experience composed of three acts. Act 1 animates materials, lights, and lens flares to reveal the Unity logo; Acts 2 and 3 present UI Toolkit layouts where you can inject logos and a paragraph. The timeline exposes events when each act starts and when the sequence ends, supports global slow‑motion and smoothing, and lets players skip with any key when you’re not loading scenes.

![screenshot](Documentation/Screenshot.png)

## Features
- One‑click prefab
  - Menu: GameObject → Essentials → Splash Screen
  - Spawns `UnityEssentials_Prefab_SplashScreen` with all required objects wired up
- Three‑act timeline (runtime)
  - Per‑act game objects/documents and durations
  - Global tempo via `Slow Motion Multiplier` and curve blending via `Smooth Strength`
  - Optional skip: any key finalizes when not loading
- Material animation
  - Fades and pulses via `_Exposure_Map_Strength` and `_Exposure_Mask_Falloff_Strength`
  - Helpers to change exposure, falloff, and animate over time with smoothing
- Lights and lens flares
  - White/green point lights fade in/out with configurable target intensities
  - Lens flare scale/position animation and intensity sync to the green light
- Audio sync
  - AudioSource pitch compensated by slow‑motion; playback spans the sequence
- UI Toolkit branding
  - Acts 2–3 use `UIDocument` UXML
  - Textures and text are injected at runtime into named elements:
    - `TopLeftLogo`, `LeftLogo`, `RightLogo` (Act 2)
    - `Logo1`, `Logo2`, `Logo3`, `Logo4`, and `Paragraph` (Act 3)
- Events API
  - `OnAct1Enabled`, `OnAct2Enabled(UIDocument)`, `OnAct3Enabled(UIDocument)`, and `OnFinalization()` via UnityEvents
  - `IsLoadingScenes` flag gates skipping and finalization while you async‑load the next scene

## Requirements
- Unity Editor 6000.0+
- UI Toolkit (`UIDocument` for Act 2/3 UXML)
- Materials for the logo/glow with properties:
  - `_Exposure_Map_Strength`, `_Exposure_Mask_Falloff_Strength`
- Optional (highly recommended): SRP Lens Flares
  - Assign `LensFlareComponentSRP` for logo and green light if you want the flare effects
- Audio: an `AudioSource` with your splash SFX/music

Tip: If you plan to async‑load your main scene, set `IsLoadingScenes = true` before you start loading and set it to `false` right before you want the splash to finalize.

## Usage
1) Create the Splash Screen
- Menu: GameObject → Essentials → Splash Screen
- Or drag `Resources/UnityEssentials_Prefab_SplashScreen.prefab` into your bootstrap scene

2) Wire references on `SplashScreenTimeline`
- Materials: Unity Label/Logo/Logo Outline/Glow Triangle (must have the exposure properties above)
- Lights: White and Green point lights and their target intensities
- Lens flares: assign both `Logo` and `Green Point Light` lens flares (SRP)
- Audio: assign an `AudioSource`
- Durations: set `Act 1/2/3 Duration` (seconds)
- Settings: tune `Slow Motion Multiplier` and `Smooth Strength`

3) Customize visuals with `SplashScreenCustomizer`
- Textures: Top‑left logo, left/right logos (Act 2), small logos 1–4 (Act 3)
- Paragraph: enter your description text for Act 3
- Events: hook `Act 1`, `Act 2`, `Act 3`, and `Final` UnityEvents to trigger your logic (e.g., start scene load, fade audio)
- Loading gate: toggle `IsLoadingScenes` while you load so the splash doesn’t skip/finish early

4) Run
- The timeline starts on Play
- Press any key to skip if `IsLoadingScenes` is false; otherwise it waits until you clear the flag and then finalizes

## How It Works
- Timeline (coroutines)
  - Resets initial state, then schedules staggered animations:
    - Material exposure/falloff changes, light intensity fades, lens flare size/position animations
  - Uses a blended interpolation: `t` is mixed between linear and smoothstep via a `Smooth Strength` overdrive
  - Adjusts `AudioSource.pitch` by the slow‑motion multiplier and plays once
  - Activates Act 1 → Act 2 → Act 3 game objects and invokes `OnActXEnabled` on the customizer with the relevant `UIDocument`
  - Waits `ActXDuration * SlowMotionMultiplier` between acts; after Act 3 waits while `IsLoadingScenes` is true; then invokes `OnFinalization`
- Customizer (UI injection)
  - On Act 2/3 enable, queries the `UIDocument.rootVisualElement` by name and assigns textures/text
  - Named elements expected:
    - Act 2: `TopLeftLogo`, `LeftLogo`, `RightLogo`
    - Act 3: `TopLeftLogo`, `Logo1`, `Logo2`, `Logo3`, `Logo4`, `Paragraph`

## Notes and Limitations
- Materials
  - If a material is missing or doesn’t have the required properties, a warning is logged and that animation is skipped
- Lens flares
  - Intensity sync in `Update()` expects both lens flares to be assigned when the green light is used; either assign both or remove the references
- Input skip
  - Skipping is disabled while `IsLoadingScenes` is true; clear it right before you want to transition
- UXML contract
  - If you replace the provided UXML, keep the element names listed above or adapt the customizer code
- Namespaces
  - Runtime scripts reside in the `Unity.Essentials` namespace (note the dot)

## Files in This Package
- Runtime
  - `Runtime/SplashScreenTimeline.cs` – Three‑act sequence, animations, audio, input skip, finalization
  - `Runtime/SplashScreenCustomizer.cs` – Texture/text injection for Acts 2–3; UnityEvents for acts and finalization
  - `Runtime/UnityEssentials.UIToolkitSplashScreen.asmdef`
- Editor
  - `Editor/SplashScreenPrefabSpawner.cs` – Menu: GameObject → Essentials → Splash Screen (spawns prefab)
  - `Editor/UnityEssentials.UIToolkitSplashScreen.Editor.asmdef`
- Resources (used by the prefab)
  - `Resources/UnityEssentials_Prefab_SplashScreen.prefab`
  - Supporting assets under `Resources/` (Audios, Logos, LensFlares, Shaders, UIToolkit, Profiles, Unity Label, Unity Logo)

## Tags
unity, splash screen, startup, ui toolkit, uxml, lens flare, materials, exposure, lights, audio, coroutine, timeline, prefab
