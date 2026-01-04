using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAdventures.Src.Interfaces
{
    public partial class DevGuiRenderer : Overlay
    {
        public DevGuiRenderer() : base("Dungeon Adventures DevGui", true)
        {
            LoadTheme();
        }

        private void LoadTheme()
        {
            // Simple theme setup - can be expanded
            ImGuiStylePtr style = ImGui.GetStyle();
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.1f, 0.13f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.4f, 0.2f, 0.2f, 1.0f);
            style.Colors[(int)ImGuiCol.Border] = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        }

        public void Toggle()
        {
            IsVisible = !IsVisible;
        }

        protected override void Render()
        {
            CheckInput();

            if (IsVisible)
            {
                ImGui.SetNextWindowSize(new Vector2(800, 600), ImGuiCond.FirstUseEver);
                if (ImGui.Begin("DevTools", ref _isVisible)) // using backing field if property exists, wait. DevGui_Vars has IsVisible property.
                {
                    // Update the property based on ref (if ref matches property, but ref needs field)
                    // ImGui.Begin takes 'ref bool p_open'. 
                    // Since IsVisible is a property in DevGui_Vars.cs, I need to use a local bool or direct field if accessible.
                    // DevGui_Vars.cs: public bool IsVisible { get; set; } = false;
                    // I can't pass a property as ref.
                    bool open = IsVisible;
                    
                    if (ImGui.BeginTabBar("DevTabs"))
                    {
                        if (ImGui.BeginTabItem("System"))
                        {
                            RenderDevLog();
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Adventures"))
                        {
                            RenderAdventures();
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("World"))
                        {
                            RenderWorld();
                            ImGui.EndTabItem();
                        }
                         if (ImGui.BeginTabItem("Forge"))
                        {
                            RenderForge();
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Specialization"))
                        {
                            RenderSpecialization();
                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }
                    
                    // Sync back visibility if closed from UI x button
                    IsVisible = open; 
                    
                    ImGui.End();
                }
                
                // If the user closed the window via 'x', IsVisible should be false.
                // But ImGui.Begin returns false if collapsed or closed? 
                // Actually ImGui.Begin(name, ref open) updates open.
                // I need to update IsVisible.
                // Re-doing logic:
                bool visible = IsVisible;
                if(visible) {
                    if (!ImGui.Begin("DevTools", ref visible))
                    {
                        // Collapsed or closed
                        ImGui.End();
                    }
                    else
                    {
                         // ... Content ...
                         if (ImGui.BeginTabBar("DevTabs"))
                        {
                            if (ImGui.BeginTabItem("System"))
                            {
                                RenderDevLog();
                                ImGui.EndTabItem();
                            }
                            if (ImGui.BeginTabItem("Adventures"))
                            {
                                RenderAdventures();
                                ImGui.EndTabItem();
                            }
                            if (ImGui.BeginTabItem("World"))
                            {
                                RenderWorld();
                                ImGui.EndTabItem();
                            }
                            if (ImGui.BeginTabItem("Forge"))
                            {
                                RenderForge();
                                ImGui.EndTabItem();
                            }
                            if (ImGui.BeginTabItem("Specialization"))
                            {
                                RenderSpecialization();
                                ImGui.EndTabItem();
                            }

                            ImGui.EndTabBar();
                        }
                        ImGui.End();
                    }
                }
                IsVisible = visible;
            }
        }

        private void CheckInput()
        {
            // Simple F8 toggle logic
            // NOTE: GetAsyncKeyState is Windows specific. 
            // For Linux, we might need a different approach or this will throw/be ignored.
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // VK_F8 = 0x77
                    short keyState = GetAsyncKeyState(0x77); 
                    bool isPressed = (keyState & 0x8000) != 0;

                    if (isPressed && (DateTime.Now - _lastF8ToggleTime).TotalSeconds > 0.2)
                    {
                        IsVisible = !IsVisible;
                        _lastF8ToggleTime = DateTime.Now;
                    }
                }
            }
            catch
            {
                // Ignore platform errors
            }
        }
        
        // Backing field for ImGui ref if needed, but IsVisible is property in DevGui_Vars.
        private bool _isVisible; // Shadowing? No, let's use local var in Render.
    }
}