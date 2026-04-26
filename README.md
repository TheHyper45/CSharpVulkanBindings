# C# Vulkan Bindings Generator

## Description
This repository contains a source generator (`CSharpVulkanBindings` project) that consumes <a href="https://github.com/KhronosGroup/Vulkan-Docs/blob/main/xml/vk.xml">`vk.xml`</a> and produces Vulkan bindings with some additional features.

The generator produces two files:
- `Vulkan.g.cs`:<br>
The file contains a single static class `VulkanBindings` inside `TheHyper45.CSharpVulkanBindings` namespace. The class contains all Vulkan constants, structures and functions.
- `VulkanAux.g.cs`:<br>
The file contains a single static class `VulkanBindingUtils` inside `TheHyper45.CSharpVulkanBindings` namespace. The class contains a `VulkanFormatByteSize` function that takes a `VkFormat` and returns its size in bytes, `VulkanGlobalCommands` class that loads global commands and `VulkanInstanceCommands` that loads instance commands.

Classes for loading commands take a pointer to `vkGetInstanceProcAddr` without caring how the comsumer obtained it (they don't load any native libraries as that is delegated to the consumer).

## Importing
Since I haven't made any nuget packages (I might do that in the future), most likely the best options is to use a git submodule to import this project into yours.

Add these tags to the `.csproj` file of the consuming project:
```XML
<ItemGroup>
  <AdditionalFiles Include="PATH/TO/PROJECT/vk.xml" />
  <ProjectReference Include="PATH/TO/PROJECT/CSharpVulkanBindings.csproj" ReferenceOutputAssembly="false" OutputItemType="Analyzer">
  </ProjectReference>
</ItemGroup>
```

The consuming project needs to have enabled `unsafe` blocks. To enable `unsafe` code add this tag to `<PropertyGroup>` tag:
```XML
<AllowUnsafeBlocks>true</AllowUnsafeBlocks>
```

When using git submodules, make sure to compile the generator first, otherwise it won't work.

## Example usage
Project `CSharpVulkanBindingsTests` contains a simple program that uses the bindings.
