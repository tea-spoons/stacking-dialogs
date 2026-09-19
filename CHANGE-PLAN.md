# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.stacking-dialogs` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- The samples folder now uses Unity's hidden `Samples~` layout and is registered in `package.json`.
- Declared the missing dependency on `addressables-toolbox`.
- Made standalone: no longer declares UniTask as a dependency. Without UniTask a second, synchronous path instantiates prefabs and shows or hides dialogs immediately (no animations). Added PlayMode tests that run in both modes.
- Restored the `.meta` files inside `Samples~`: the first migration dropped them, which breaks the links between sample assets when a sample is imported.
- 0.18.0: `com.tea-spoons.package-core` is no longer a dependency. The package uses it when the project has it (`TEASPOONS_PACKAGE_CORE`, set from the asmdef `versionDefines`) and otherwise its own small copies: `PlayModeEditable<T>` (the wrapper of the `DialogSpace` settings, same serialized `value` layout), its inspector drawer and `SerializedProperty.TryGetTargetObject` (new `TeaSpoons.StackingDialogs.Editor` assembly), and the menu root string. Added EditMode tests that guard the serialized layout and a PlayMode test for the settings update events.

## Planned changes

- [x] Tag and publish `v0.17.1` with the Release workflow.
- [ ] Tag and publish `v0.18.0` with the Release workflow.
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.
- [ ] Look at two things noticed while reading the code: `DialogSpace.OnDisable` compares with `=` instead of `==` (it clears the default space whenever any space is disabled), and with UniTask `CloseAll` seems to raise `RemovedLast` twice.

## Notes and ideas

_Add your own here._
