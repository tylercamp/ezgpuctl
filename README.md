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
- Overclocking profile config page
- Ability to apply per-GPU overclocking profiles
- Ability to configure overclocking profile for more than 1 GPU
- Ability to apply multi-GPU overclocking profiles
- Ability to set an OC profile depending on running processes
- Ability to compose OC profiles depending on use of multiple processes
- Fan control