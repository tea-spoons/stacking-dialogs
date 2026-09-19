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
- [x] Tag and publish `v0.18.0` with the Release workflow.
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.
- [ ] Look at two things noticed while reading the code: `DialogSpace.OnDisable` compares with `=` instead of `==` (it clears the default space whenever any space is disabled), and with UniTask `CloseAll` seems to raise `RemovedLast` twice.
<!-- review-items:start -->
- [ ] **P0** Fix the `=` versus `==` bug in `DialogSpace.OnDisable` and the double `RemovedLast`, each with a regression test (existing item, now split and prioritized).
- [ ] **P1** Make `unitask-toolbox` and `addressables-toolbox` optional. That would also let the package install without package-core, which today arrives through addressables-toolbox.
- [ ] **P1** Declares `unity: 6000.0`, but only Unity 6000.3.8f1 was tested. Add a Unity version matrix to CI once package tests run there (see the `unity-ci-kit` plan), or raise the minimum.
- [ ] **P1** Run the tests in CI. The kit's `run-tests` needs a Unity project, so this waits for package-mode support in `unity-ci-kit` (planned there; GameCI's test runner has a `packageMode` for the same reason).
- [ ] **P2** Route the messages through a small `Conditional` wrapper (as `ams` does), so release builds do not carry them.
- [ ] **P2** Document the difference between stacking and non-stacking spaces with a sequence diagram, and describe the samples in a README.
- [ ] **P2** Add a `CHANGELOG.md`. Unity's package layout lists one next to `README.md`, and the `unity-ci-kit` validator warns without it.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Unity UI Extensions (uGUI)](https://github.com/Unity-UI-Extensions/com.unity.uiextensions) | BSD-3-Clause (uGUI package) | A large catalogue of uGUI controls. No dialog-stack comparable turned up in this pass. |

### Findings from reading the code

- **[Bug]** `DialogSpace.OnDisable` uses `if (DefaultSpace = this)` (an assignment, not a comparison), so disabling any space clears the default space. This is already in the plan; it is a P0 because it changes behavior in normal use.
- **[Bug]** With UniTask, `CloseAll` seems to raise `RemovedLast` twice (already in the plan).
- **[Coupling]** It still declares `unitask-toolbox` and `addressables-toolbox`, although the package already has a synchronous path without UniTask and compiles the Addressables-backed `DialogReference` out when the packages are missing.
- **[Logging]** Four direct `Debug.Log*` calls in the runtime code. Logging is now an optional dependency elsewhere in the library, so the same pattern can be used here.
<!-- review:end -->

## Notes and ideas

_Add your own here._
