# EZ GPU Control

A WIP tool for managing GPU OCs, which can automatically change GPU OCs depending on what programs are running.

Eg:

- I want max power target when running a game
- I want 70% power target when running BOINC
- Otherwise, use default power target

*This is currently NVIDIA-only.*

## Usage

Download the most recent `gpucontrol.zip` file from this repo's Releases section. Extract the _entire_ ZIP file.

Run the app and accept its terms. The main window will allow you to add profiles which specify OC settings. Profiles can optionally define 1 more more setting(s) on a per-GPU basis. After adding at
least 1 profile, you can add a policy which will be applied when the configured programs are running. If multiple policies are active, the OCs will be merged in reverse order, eg the top-most policy
will have priority. The profiles within a policy are merged with the same logic.

When running, the app will have an icon in the system tray. Right-click this for various settings, such as run-on-startup and changing the OC mode (policies-based, specific policy, specific profile).

Hover over the tray icon for info on the OC settings currently being applied.

## Known Issues

- Window controls may flicker when the UI refreshes
- Minimize-on-start may not fully minimize the window

Any other bugs should be reported as an Issue on this GitHub repository. If the app is crashing, include the contents of `log.txt`.

## Limitations

These features are not offered:

- VF curves - These are currently unsupported by NvAPIWrapper
- Fan curves - I don't want to bother with a curve editor
- Voltage offsets (%) - These seem to be placebo

Testing has only been done on 3000-series cards.

The impl. details for NVIDIA are handled by [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper). Adding support for AMD GPUs should only require appropriate changes to the `GPUControl.Lib`
package, namely static methods in `IGpuWrapper`. I tried AMD's ADLX but [ran into a snag](https://github.com/GPUOpen-LibrariesAndSDKs/ADLX/issues/1) preventing me from using it. ADL should work,
just don't have the patience to write a wrapper right now.

## TODOs

- ~Basic enumeration of GPUs + realtime display~
- ~Overclocking profile config page~
- ~Ability to apply per-GPU overclocking profiles~
- ~Ability to configure overclocking profile for more than 1 GPU~
- ~Ability to apply multi-GPU overclocking profiles~
- ~Ability to set an OC profile depending on running processes~
- ~Ability to compose OC profiles depending on use of multiple processes~
- ~Add copy of LGPL license to release files for NvAPIWrapper~
- ~Add more details to GPU status view for identifying multiple GPUs~
- ~Toggle to run on startup~
- ~Buttons for controlling policy service~
- ~System tray entry for starting/stopping the policy service~
- General UI improvements
- ~GPU impl. abstraction~
- ~Fan control~ (not properly tested)
- Support for temperature limits in policies
- Proper error handling, etc. (bare basics are handled)