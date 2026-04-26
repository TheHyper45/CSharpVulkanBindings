using System.Runtime.InteropServices.Marshalling;
using static TheHyper45.CSharpVulkanBindings.VulkanBindings;
using static TheHyper45.CSharpVulkanBindings.VulkanBindingUtils;

namespace TheHyper45.CSharpVulkanBindingTests;

public sealed class VulkanPhysicalDevice
{
  private VkPhysicalDevice physicalDevice = VkPhysicalDevice.Null;
  private VkPhysicalDeviceProperties physicalDeviceProperties = new();

  public unsafe VulkanPhysicalDevice(VulkanInstanceCommands commands,VkPhysicalDevice _physicalDevice)
  {
    physicalDevice = _physicalDevice;
    fixed(VkPhysicalDeviceProperties* ptr = &physicalDeviceProperties)
    {
      commands.vkGetPhysicalDeviceProperties(physicalDevice,ptr);
    }
  }

  public unsafe string Name
  {
    get
    {
      fixed(byte* ptr = physicalDeviceProperties.deviceName)
      {
        return Utf8StringMarshaller.ConvertToManaged(ptr) ?? throw new NullReferenceException();
      }
    }
  }
}
