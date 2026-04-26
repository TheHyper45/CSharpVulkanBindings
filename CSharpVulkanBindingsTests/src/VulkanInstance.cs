using static TheHyper45.CSharpVulkanBindings.VulkanBindings;
using static TheHyper45.CSharpVulkanBindings.VulkanBindingUtils;

namespace TheHyper45.CSharpVulkanBindingTests;

public sealed class VulkanInstance : IDisposable
{
  private VkInstance instance = VkInstance.Null;
  public VulkanInstanceCommands Dispatch { get; private set; }

  public unsafe VulkanInstance(PFN_vkGetInstanceProcAddr vkGetInstanceProcAddr)
  {
    var commands = new VulkanGlobalCommands(vkGetInstanceProcAddr);
    VkApplicationInfo applicationInfo = new()
    {
      sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
      apiVersion = VK_API_VERSION_1_0
    };
    VkInstanceCreateInfo instanceCreateInfo = new()
    {
      pApplicationInfo = &applicationInfo
    };
    fixed(VkInstance* ptr = &instance)
    {
      var result = commands.vkCreateInstance(&instanceCreateInfo,null,ptr);
      if(result != VkResult.VK_SUCCESS)
      {
        throw new ApplicationException($"`vkCreateInstance` returned `{result}`.");
      }
    }
    Dispatch = new(vkGetInstanceProcAddr,instance);
  }

  public unsafe void Dispose()
  {
    Dispatch.vkDestroyInstance(instance,null);
    instance = VkInstance.Null;
  }

  public unsafe VulkanPhysicalDevice[] GetPhysicalDevices()
  {
    uint count = 0;
    var result = Dispatch.vkEnumeratePhysicalDevices(instance,&count,null);
    if(result != VkResult.VK_SUCCESS)
    {
      throw new ApplicationException($"`vkEnumeratePhysicalDevices` returned `{result}`.");
    }
    var physicalDevices = new VkPhysicalDevice[count];
    fixed(VkPhysicalDevice* ptr = physicalDevices)
    {
      result = Dispatch.vkEnumeratePhysicalDevices(instance,&count,ptr);
      if(result != VkResult.VK_SUCCESS)
      {
        throw new ApplicationException($"`vkEnumeratePhysicalDevices` returned `{result}`.");
      }
    }
    return [..Enumerable.Range(0,(int)count).Select(i => new VulkanPhysicalDevice(Dispatch,physicalDevices[i]))];
  }
}
