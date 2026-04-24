# C# Vulkan Bindings Generator

## Description
This project is a source generator that consumes <a href="https://github.com/KhronosGroup/Vulkan-Docs/blob/main/xml/vk.xml">`vk.xml`</a> and produces a file named `Vulkan.g.cs` that contains a single static partial class `Vulkan` inside `TheHyper45.CSharpVulkanBindings` namespace.

The reason for me making this project is simple: all other C# Vulkan bindings libraries I could find presume the way of loading the Vulkan system shared library. I just wanted a library that takes a pointer to `vkGetInstanceProcAddr` and loads all pointers to Vulkan commands for me without caring how I obtained `vkGetInstanceProcAddr`.

- All Vulkan structures, constants and function pointers are within the `Vulkan` class.
- There is a `VulkanFormatByteSize` static method that takes `VkFormat` and returns its size in bytes.
- There are two classes for loading function pointers: `VulkanGlobalOnlyDispatch` and `VulkanInstanceOnlyDispatch`. `VulkanGlobalOnlyDispatch` only takes a pointer to `vkGetInstanceProcAddr` and `VulkanInstanceOnlyDispatch` additionally takes a `VkInstance`.

## Importing
Since I haven't made any nuget packages, most likely the best options is to use a git submodule to import this project into yours.

Add these tags to the `.csproj` file of the consuming project:
```XML
<ItemGroup>
  <AdditionalFiles Include="PATH/TO/PROJECT/vk.xml" />
  <ProjectReference Include="PATH/TO/PROJECT/CSharpVulkanBindings.csproj" ReferenceOutputAssembly="false" OutputItemType="Analyzer">
  </ProjectReference>
</ItemGroup>
```

The consuming project need to have enabled `unsafe` blocks. To enable `unsafe` code add this tag to `<PropertyGroup>` tag:
```XML
<AllowUnsafeBlocks>true</AllowUnsafeBlocks>
```

## Example usage
```C#
// Uses implicit usings.
using System.Runtime.InteropServices;
using static TheHyper45.CSharpVulkanBindings.Vulkan;

unsafe
{
    var dll = NativeLibrary.Load(OperatingSystem.IsWindows() ? "vulkan-1.dll" : "vulkan-1.so");
    if(dll == nint.Zero)
    {
        Console.Error.WriteLine("Could not load `vulkan-1.dll`.");
        return;
    }

    PFN_vkGetInstanceProcAddr vkGetInstanceProcAddr = null;
    {
        var export = NativeLibrary.GetExport(dll,"vkGetInstanceProcAddr");
        if(export == nint.Zero)
        {
            Console.Error.WriteLine("Could not load `vkGetInstanceProcAddr`.");
            NativeLibrary.Free(dll);
            return;
        }
        vkGetInstanceProcAddr = (PFN_vkGetInstanceProcAddr)export;
    }

    var gDispatch = new VulkanGlobalOnlyDispatch(vkGetInstanceProcAddr);
    {
        uint version = 0;
        var result2 = gDispatch.vkEnumerateInstanceVersion(&version);
        if(result2 != VkResult.VK_SUCCESS)
        {
            Console.Error.WriteLine($"`vkEnumerateInstanceVersion` returned `{result2}`.");
            NativeLibrary.Free(dll);
            return;
        }

        var variant = VK_API_VERSION_VARIANT(version);
        var major = VK_API_VERSION_MAJOR(version);
        var minor = VK_API_VERSION_MINOR(version);
        var patch = VK_API_VERSION_PATCH(version);
        Console.WriteLine($"Instance version {variant}.{major}.{minor}.{patch}.");
    }

    VkApplicationInfo applicationInfo = new()
    {
        sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
        apiVersion = VK_API_VERSION_1_0
    };

    VkInstanceCreateInfo instanceCreateInfo = new()
    {
        sType = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
        pApplicationInfo = &applicationInfo
    };

    var instance = VkInstance.Null;
    if(instance != VkInstance.Null)
    {
        Console.WriteLine("Yes");
    }
    else
    {
        Console.WriteLine("No");
    }

    var result = gDispatch.vkCreateInstance(&instanceCreateInfo, null, &instance);
    if(result != VkResult.VK_SUCCESS)
    {
        Console.Error.WriteLine($"`vkCreateInstance` returned `{result}`.");
        NativeLibrary.Free(dll);
        return;
    }

    var dispatch = new VulkanInstanceOnlyDispatch(vkGetInstanceProcAddr, instance);

    dispatch.vkDestroyInstance(instance, null);

    NativeLibrary.Free(dll);
}
```
