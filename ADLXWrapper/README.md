(1/9/2023)

Classes exported using the ADLX C# sample project:
https://github.com/GPUOpen-LibrariesAndSDKs/ADLX/tree/main/Samples/csharp

The `ADLXCSharpBind.i` file was updated with the version included in this folder.

This project includes a copy of the compiled Release DLL from that change. Wrappers were created for
any classes that seemed relevant for overclocking.

The project has a reference to `amdadlx64.dll` not included here (due to [license terms](https://github.com/GPUOpen-LibrariesAndSDKs/ADLX/blob/main/ADLX%20SDK%20License%20Agreement.pdf))
but can be downloaded here: https://download.amd.com/dir/bin/amdadlx64.dll/

I used the [Jan 9 2023](https://download.amd.com/dir/bin/amdadlx64.dll/63AAB932421000/) build.

The DLL needs to be [downloaded manually](https://github.com/GPUOpen-LibrariesAndSDKs/ADLX/issues/4) (at time of writing).