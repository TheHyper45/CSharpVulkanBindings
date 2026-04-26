using System.Runtime.InteropServices;
using static TheHyper45.CSharpVulkanBindings.VulkanBindings;

namespace TheHyper45.CSharpVulkanBindingTests;

public sealed class VulkanLibraryLoader : IDisposable
{
  private nint handle = nint.Zero;
  private PFN_vkGetInstanceProcAddr _vkGetInstanceProcAddr = null;
  public PFN_vkGetInstanceProcAddr vkGetInstanceProcAddr => _vkGetInstanceProcAddr;

  public unsafe VulkanLibraryLoader(string? _name = null)
  {
    string? name = _name;
    if(name == null)
    {
      if(OperatingSystem.IsWindows())
      {
        name = "vulkan-1.dll";
      }
      else if(OperatingSystem.IsLinux())
      {
        name = "vulkan-1.so";
      }
      else
      {
        throw new InvalidOperationException("Unsupported operating system.");
      }
    }

    handle = NativeLibrary.Load(name);
    if(handle == nint.Zero)
    {
      throw new InvalidOperationException($"Could not load `{name}`.");
    }

    _vkGetInstanceProcAddr = (PFN_vkGetInstanceProcAddr)NativeLibrary.GetExport(handle,"vkGetInstanceProcAddr");
    if(_vkGetInstanceProcAddr.ptr == null)
    {
      throw new InvalidOperationException($"Could not load `vkGetInstanceProcAddr` from `{name}`.");
    }
  }

  public void Dispose()
  {
    NativeLibrary.Free(handle);
    handle = nint.Zero;
  }
}
