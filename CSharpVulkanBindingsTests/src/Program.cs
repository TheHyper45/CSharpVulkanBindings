using TheHyper45.CSharpVulkanBindingTests;
using static TheHyper45.CSharpVulkanBindings.VulkanBindings;
using static TheHyper45.CSharpVulkanBindings.VulkanBindingUtils;

using var vulkan = new VulkanLibraryLoader();
unsafe
{
  var globalCommands = new VulkanGlobalCommands(vulkan.vkGetInstanceProcAddr);
  uint version = 0;
  var result = globalCommands.vkEnumerateInstanceVersion(&version);
  if(result != VkResult.VK_SUCCESS)
  {
    return;
  }

  uint variant = VK_API_VERSION_VARIANT(version);
  uint major = VK_API_VERSION_MAJOR(version);
  uint minor = VK_API_VERSION_MINOR(version);
  uint patch = VK_API_VERSION_PATCH(version);
  Console.WriteLine($"Instance version = `{variant}.{major}.{minor}.{patch}`.");
}

using var instance = new VulkanInstance(vulkan.vkGetInstanceProcAddr);
var physicalDevices = instance.GetPhysicalDevices();
Console.WriteLine(physicalDevices.Length == 1 ? "Found 1 physical device." : $"Found {physicalDevices.Length} physical devices.");
foreach(var physicalDevice in physicalDevices)
{
  Console.WriteLine(physicalDevice.Name);
}
