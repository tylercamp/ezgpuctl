# EZ GPU Control

A WIP tool for manging GPU OCs, with a primary eventual feature of saving OC profiles and applying them depending on the current running process.

Eg:

- I want max power target when running a game
- I want 70% power target when running BOINC
- Otherwise, use default power target

I only have NVIDIA GPUs and am only intending for this to work with NVIDIA GPUs. The deciding limitation affecting compatibility is whether there is an API available for reading and applying the relevant settings.

The impl. details for NVIDIA are handled by [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper). The code is not written to easily swap out with a different API provider, but it should be straightforward to do if/when that happens.


TODOs:

- ~Basic enumeration of GPUs + realtime display~
- ~Overclocking profile config page~
- ~Ability to apply per-GPU overclocking profiles~
- ~Ability to configure overclocking profile for more than 1 GPU~
- ~Ability to apply multi-GPU overclocking profiles~
- ~Ability to set an OC profile depending on running processes~
- ~Ability to compose OC profiles depending on use of multiple processes~
- Add copy of LGPL license to release files for NvAPIWrapper
- Add more details to GPU status view for identifying multiple GPUs
- Toggle to run on startup
- Buttons for controlling policy service
- System tray entry for starting/stopping the policy service
- General UI improvements
- GPU impl. abstraction
- Fan control

## Limitations

These features are not offered:

- VF curves - These are currently unsupported by NvAPIWrapper
- Voltage offsets (%) - These seem to be placebo
- Fan curves - My GPUs are water cooled and I cannot test fan settings

Testing has only been done on 3000-series cards.

---



---

Overclocking options are split into:

- Overclocks - Direct, per-GPU OC options
- Overclock Profiles - A list of Overclocks (1 per GPU)
- Overclock Policies - A list of program rules (program is/is not running) + an ordered list of Profiles

Overclocks can contain certain options but leave others disabled. This is so that OC settings can be merged if multiple settings are in
use, since multiple OC Profiles may be active simultaneously. When OCs are merged (priority based on their order in the profile/policy list),
OC settings which are defined will overwrite previous values for that setting. If a setting is undefined, that OC won't affect that
particular setting at all. For example:

- Profile "Compute" - Overclocks specifying 80% power target for all GPUs, other settings disabled
- Profile "Gaming" - Overclocks specifying 105% power target for GPU 1
- Profile "Safe Memory OC" - +600MHz Memory OC on GPU 1
- Profile "High Memory OC" - +1000 OC on GPU 1
- Profile "Stock" (built-in) - All settings enabled and set to stock values for all GPUs

- Policy "Folding@Home" runs when FAH exe is running, applies "Compute" profile
- Policy "Starcraft" runs when Starcraft exe is running, applies "Gaming", "Safe Memory OC" profiles
- Policy "CoD" runs when CoD exe is running, applies "Gaming", "High Memory OC" profiles
- Policy "Default" (built-in) is always active, applies "Stock" profile

With the given Policies and Profiles (in the given order)

_When playing Starcraft_ - "Gaming" (GPU 1: 105% Power) and "Safe Memory OC" (GPU 1: Memory +600MHz) are combined for an effective `GPU 1: 105% Power, Memory +600Mhz` OC.
_When playing Starcraft with FAH_ - Starcraft Policy (`GPU 1: 105% Power, Memory +600MHz`) is partially overwritten by Folding@Home Policy (`GPU 1: 80% Power`, `GPU 2: 80% Power`) giving an effective `GPU 1: 80% Power, Memory +600MHz`, `GPU 2: 80% Power`
